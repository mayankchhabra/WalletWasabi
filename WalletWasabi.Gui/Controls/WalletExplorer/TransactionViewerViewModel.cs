using Avalonia;
using Avalonia.Controls;
using NBitcoin;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WalletWasabi.Models;

namespace WalletWasabi.Gui.Controls.WalletExplorer
{
	public class TransactionViewerViewModel : WalletActionViewModel
	{
		private CompositeDisposable Disposables { get; set; }

		private string _txid;
		private string _psbtJsonText;
		private string _psbtHexText;
		private string _psbtBase64Text;
		private byte[] _psbtBytes;
		public ReactiveCommand<Unit, Unit> ExportBinaryPsbtCommand { get; set; }

		public string TxId
		{
			get => _txid;
			set => this.RaiseAndSetIfChanged(ref _txid, value);
		}

		public string PsbtJsonText
		{
			get => _psbtJsonText;
			set => this.RaiseAndSetIfChanged(ref _psbtJsonText, value);
		}

		public string TransactionHexText
		{
			get => _psbtHexText;
			set => this.RaiseAndSetIfChanged(ref _psbtHexText, value);
		}

		public string PsbtBase64Text
		{
			get => _psbtBase64Text;
			set => this.RaiseAndSetIfChanged(ref _psbtBase64Text, value);
		}

		public byte[] PsbtBytes
		{
			get => _psbtBytes;
			set => this.RaiseAndSetIfChanged(ref _psbtBytes, value);
		}

		public TransactionViewerViewModel(WalletViewModel walletViewModel) : base("Transaction", walletViewModel)
		{
			ExportBinaryPsbtCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				try
				{
					var sfd = new SaveFileDialog {
						DefaultExtension = "psbt",
						InitialFileName = TxId,
						Title = "Export Binary PSBT"
					};

					string file = await sfd.ShowAsync(Application.Current.MainWindow);
					if (!string.IsNullOrWhiteSpace(file))
					{
						await File.WriteAllBytesAsync(file, PsbtBytes);
					}
				}
				catch (Exception ex)
				{
					SetWarningMessage(ex.ToTypeMessageString());
					Logging.Logger.LogError<TransactionViewerViewModel>(ex);
				}
			}, outputScheduler: RxApp.MainThreadScheduler);
		}

		private void OnException(Exception ex)
		{
			SetWarningMessage(ex.ToTypeMessageString());
		}

		public override void OnOpen()
		{
			if (Disposables != null)
			{
				throw new Exception("Transaction Viewer was opened before it was closed.");
			}

			Disposables = new CompositeDisposable();

			base.OnOpen();
		}

		public override bool OnClose()
		{
			Disposables?.Dispose();
			Disposables = null;

			return base.OnClose();
		}

		public void Update(BuildTransactionResult result)
		{
			try
			{
				TxId = result.Transaction.GetHash().ToString();
				PsbtJsonText = result.Psbt.ToString();
				TransactionHexText = result.Transaction.Transaction.ToHex();
				PsbtBase64Text = result.Psbt.ToBase64();
				PsbtBytes = result.Psbt.ToBytes();
			}
			catch (Exception ex)
			{
				OnException(ex);
			}
		}
	}
}
