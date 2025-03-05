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

[Tool]
[GlobalClass]
public partial class DialogRayCast3d : RayCast3D
{
    private IDialogueable? currentDialogueable;

    [Export]
    public bool IsConfigurated { get; set; } = false; // This property is used to remove the warning message


	public override void _Ready()
	{
        SetCollisionMaskValue(IDialogueable.DIALOG_LAYER, true);
        CollideWithBodies = true;
        CollideWithAreas = true;

		if (!Engine.IsEditorHint())
		{
			DialogUI.Instance.DialogStarted += EnterDialogMode;
			DialogUI.Instance.DialogFinished += ExitDialogMode;
		}

	}

	private void ExitDialogMode()
	{
		SetProcess(true);
	}

	private void EnterDialogMode()
	{
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		ProcessDialogColission();
	}

	public void Interact()
	{
		if (currentDialogueable is not null)
		{
			DialogUI.Instance.EmitSignal(DialogUI.SignalName.DialogStarted);
			currentDialogueable.StartDialogue(GetOwner<Node>());
		}
	}

    private void ProcessDialogColission()
	{
		if (Enabled && GetCollider() is IDialogueable newDialogueable)
		{
			try
			{
				if (newDialogueable != currentDialogueable)
				{
					TryDisableCurrentDialogueable();
				}
			}
			finally
			{
				currentDialogueable = newDialogueable;
				newDialogueable.CanStartDialogue(true);
			}
			return;
		}

		TryDisableCurrentDialogueable();
	}

	private void TryDisableCurrentDialogueable()
	{
		try
		{
			currentDialogueable?.CanStartDialogue(false);
		}
		finally
		{
			currentDialogueable = null;
		}
	}

    public override string[] _GetConfigurationWarnings()
    {
        return IsConfigurated ? [] : [
            "Remember to set the collision mask to the dialogue layer",
            "Make sure to set the collision mask with the environment layer, to avoid dialog within a wall",
            "To remove this warning, set the IsConfigurated property to true and reload the scene"
        ];
    }
}
