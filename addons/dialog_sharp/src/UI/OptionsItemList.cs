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

using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class OptionsItemList : ItemList
{
	[Signal]
    public delegate void NextPageSelectedEventHandler(DialogPageBase dialogPageBase);

	[Export]
	public AnimationPlayer animationPlayer = null!;

	[Export]
	public IndicativeLabel indicativeLabel = null!;

	private DialogPageWithOptions dialogPage = null!;

	private int selectedIndex = 0;

	public override void _Ready()
	{
		ItemClicked += OnItemClicked;
		ItemSelected += OnItemSelected;
		SetProcessInput(false);
	}

	public void Fill(DialogPageWithOptions dialogPageWithOptions)
	{
		dialogPage = dialogPageWithOptions;
		selectedIndex = 0;
		indicativeLabel.Visible = dialogPage.isOptional;

		Clear();

		for (int i = 0; i < dialogPage.Options.Length; i++)
		{
			var item = dialogPage.Options[i];
			AddItem(item.Text);
			SetItemTooltipEnabled(i, false);
		}

		ShowOptions();
	}

	private void ShowOptions()
	{
		GrabFocus();
		MouseFilter = MouseFilterEnum.Stop;
		Select(0);
		SetProcessInput(true);

		Show();
		animationPlayer.Play("Fade");
	}

	private void OnItemSelected(long index)
	{
		selectedIndex = (int) index;
	}

	public override async void _Input(InputEvent @event)
	{
		if (dialogPage.isOptional && Input.IsActionJustPressed("select_last_menu_item_dialog"))
		{
			await SelectNextPageAtIndexAsync(ItemCount - 1);
		}
		else if (Input.IsActionJustPressed("select_menu_item_dialog"))
		{
			await SelectNextPageAtIndexAsync(selectedIndex);
		}
	}
	private async void OnItemClicked(long index, Vector2 atPosition, long mouseButtonIndex)
	{
		await SelectNextPageAtIndexAsync((int) index);
	}

	private async Task SelectNextPageAtIndexAsync(int index)
	{
		Select(index);
		EmitSignal(SignalName.NextPageSelected, dialogPage.Options.ElementAt(index).NextPage!);
		await StopProcessingAsync();
	}

	public async Task StopProcessingAsync()
	{
		ReleaseFocus();
		MouseFilter = MouseFilterEnum.Ignore;
		SetProcessInput(false);
		animationPlayer.PlayBackwards("Fade");
		await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		Hide();
	}
}