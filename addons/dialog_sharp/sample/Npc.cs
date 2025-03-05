// MIT License
//
// Copyright (c) 2025 Poing Studios
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

public partial class Npc : StaticBody2D
{

    [Export]
    public TextureRect DialogIcon { get; set; } = null!;

    [Export]
    public DialogArea2D DialogArea { get; set; } = null!;

    public override void _Ready() {
        DialogArea.DialogueStarted += DialogueStarted;
        DialogArea.CanDialogue += CanDialogue;
    }

    private void CanDialogue(bool value)
    {
        DialogIcon.Visible = value;
    }
    public DialogPageWithOptions dialogPageWithOptions { get; set; } = new DialogPageWithOptions()
    {
        Title = "Gandalf",
        Body = "Select an option",
        Options = new []
        {
            new DialogMenuOptions()
            {
                Text = "I don't have next page"
            },
            new DialogMenuOptions()
            {
                Text = "I have next page",
                NextPage = new DialogPageWithNextPage()
                {
                    Title = "Gandalf",
                    Body = "You shall not pass!"
                }
            }
        }
    };
    private async void DialogueStarted(Node entity)
    {
        DialogIcon.Visible = false;
        await DialogUI.Instance.ShowDialogAsync(dialogPageWithOptions);
    }
}
