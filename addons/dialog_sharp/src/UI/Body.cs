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

public partial class Body : RichTextLabel
{
	[Signal]
	public delegate void DialogStartedEventHandler();

	[Signal]
	public delegate void ClickedOnEventHandler();

	[Signal]
	public delegate void FilledEventHandler();

	private const byte AUTO_FILL_THRESHOLD = 1;

    public bool CanGoToNextPage => VisibleRatio == AUTO_FILL_THRESHOLD;

    public new string Text
    {
        get => base.Text;
        set
        {
			/* IF needed to translate some string inner string, use this code
			//example: MY_3_TIMES_BODY_KEY_1,"FIRST AND NOT !@#MY_BODY_KEY!@# !@#MY_TITLE_KEY!@# LAST PAGE! !@#MY_TITLE_KEY!@#  !@#MY_TITLE_KEY!@#  !@#MY_TITLE_KEY!@# ","PRIMEIRA E NÃO A ÚLTIMA PÁGINA!","¡PRIMERA Y NO LA ÚLTIMA PÁGINA!"
			var originalTranslatedValue = Tr(value);
			var arrayBody = originalTranslatedValue.Split("!@#");

			var finalResult = string.Empty;
			for (int i = 0; i < arrayBody.Length; i++)
			{
				var localString = arrayBody[i];
				if (i%2 != 0)
				{
					localString = Tr(arrayBody[i]);
				}
				finalResult += localString;
			}
			*/
            base.Text = value;
            ResetTextDisplay();
        }
    }

    public override void _Ready()
    {
        SetProcess(false);
		GuiInput += MouseInput;
    }

	private void MouseInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("left_click") is false)
		{
			return;
		}
		
		EmitSignal(SignalName.ClickedOn);
	}

	private void ResetTextDisplay()
    {
        VisibleRatio = 0;
        SetProcess(true);
    }

    public void FillBody()
    {
		if (!IsProcessing()) return;

        SetProcess(false);
        VisibleRatio = AUTO_FILL_THRESHOLD;
		EmitSignal(SignalName.Filled);
    }

    public override async void _Process(double delta)
    {
        if (CanGoToNextPage)
        {
            FillBody();
            return;
        }

        VisibleCharacters++;
        await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);
    }
}
