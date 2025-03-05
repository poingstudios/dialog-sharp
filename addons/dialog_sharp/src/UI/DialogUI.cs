// MIT License
//
// Copyright (c) 2025-present Poing Studios
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Godot;
using System.Threading.Tasks;

public partial class DialogUI : PanelContainer
{
	private const string NEXT_PAGE_DIALOG_ACTION = "next_page_dialog";
	private const string SKIP_WRITTING_BODY_DIALOG_ACTION = "skip_writting_body_dialog";
	private const string FADE_ANIMATION = "Fade";
	private bool IsOnDialog { get; set; } = false;

	[Signal]
    public delegate void DialogStartedEventHandler();
	[Signal]
    public delegate void DialogFinishedEventHandler();

	#region Export Nodes
	[Export]
	public PanelContainer profilePanel = null!;
	[Export]
	public TextureRect profileIcon = null!;
	[Export]
	public Label title = null!;
	[Export]
	public Body body = null!;
	[Export]
    private RichTextLabel nextPageIndicativeLabel = null!;

	[Export]
	public OptionsItemList optionsItemList = null!;
	[Export]
	private AnimationPlayer mainBoxAnimationPlayer = null!;

	#endregion

	private DialogPageBase? nextPage;
	private DialogPageWithOptions? dialogPageWithOptions;

	public static DialogUI Instance {get; private set;} = null!;
	public bool CanGoToNextPage { get => body.CanGoToNextPage && dialogPageWithOptions is null; }

	public override void _Ready()
	{
		Hide();
		Instance = this;
		SetProcessInput(false);
		body.ClickedOn += OnClickedOnBody;
		body.Filled += OnBodyFilled;

		optionsItemList.NextPageSelected += OnOptionsNextPageSelected;
	}
	public async Task ShowDialogAsync(DialogPageBase? dialogPage)
	{
		IsOnDialog = true;
		mainBoxAnimationPlayer.Play(FADE_ANIMATION);
		Show();
		await ReadNextPageAsync(dialogPage);
		SetProcessInput(true);
	}

	public override async void _Input(InputEvent @event)
	{
		bool nextPagePressed = Input.IsActionJustPressed(NEXT_PAGE_DIALOG_ACTION);
		if (nextPagePressed || Input.IsActionJustPressed(SKIP_WRITTING_BODY_DIALOG_ACTION))
		{
			await TryReadNextPageAction(nextPagePressed);
		}
	}

	private async Task TryReadNextPageAction(bool nextPagePressed = true)
	{
		if (nextPagePressed && CanGoToNextPage)
		{
			await ReadNextPageAsync(nextPage);
		}
		else
		{
			body.FillBody();
		}
	}

	private async Task ReadNextPageAsync(DialogPageBase? dialogPage)
	{
		if (dialogPage is null){
			await FinishDialogAsync();
			return;
		}

		PreProcessDialogPage(dialogPage);

		switch (dialogPage)
		{
			case DialogPageWithNextPage dialogPageWithNextPage:
				ProcessPageWithNextPage(dialogPageWithNextPage);
				break;
			case DialogPageWithOptions dialogPageWithOptions:
				ProcessPageWithOptions(dialogPageWithOptions);
				break;
		}
	}

	public async Task FinishDialogAsync()
	{
		body.SetProcess(false);
		_ = optionsItemList.StopProcessingAsync();
		SetProcessInput(false);

		EmitSignal(SignalName.DialogFinished);
		
		if (!IsOnDialog) return;
		IsOnDialog = false;

		mainBoxAnimationPlayer.PlayBackwards(FADE_ANIMATION);
		await ToSignal(mainBoxAnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		Hide();
	}

	private void PreProcessDialogPage(DialogPageBase dialogPage)
	{
		nextPageIndicativeLabel.Visible = false;

		profileIcon.Texture = dialogPage.Profile;
		if (dialogPage.Profile is not null)
		{
			profilePanel.Show();
		}
		else
		{
			profilePanel.Hide();
		}
		
		if (!string.IsNullOrEmpty(dialogPage.Title))
		{
			title!.Text = dialogPage.Title;
			title.Show();
		}
		else
		{
			title!.Hide();
		}

		body!.Text = dialogPage.Body!;
	}

	private void ProcessPageWithNextPage(DialogPageWithNextPage dialogPageWithNextPage)
	{
		nextPage = dialogPageWithNextPage.NextPage;
		dialogPageWithOptions = null;
	}

	private void ProcessPageWithOptions(DialogPageWithOptions dialogPageWithOptions)
	{
		this.dialogPageWithOptions = dialogPageWithOptions;
	}

	private async void OnClickedOnBody()
	{
		await TryReadNextPageAction();
	}

	private async void OnOptionsNextPageSelected(DialogPageBase dialogPageBase)
	{
		SetProcessInput(true);
		await ReadNextPageAsync(dialogPageBase);
	}

	private void OnBodyFilled()
	{
		nextPageIndicativeLabel.Visible = dialogPageWithOptions is null;
		if (dialogPageWithOptions is not null)
		{
			SetProcessInput(false);
			optionsItemList.Fill(dialogPageWithOptions);
		}
	}
}
