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
[Icon("uid://dav31k1iifer1")] 
public partial class DialogArea2D : Area2D, IDialogueable
{
	[Signal]
    public delegate void DialogueStartedEventHandler(Node entity);

	[Signal]
	public delegate void CanDialogueEventHandler(bool value);

	public override void _EnterTree()
	{
		Monitoring = false;
		SetCollisionLayerValue(IDialogueable.DIALOG_LAYER, true);
		SetCollisionLayerValue(1, false);
		SetCollisionMaskValue(1, false);
	}

	public void StartDialogue(Node entity)
	{
		EmitSignal(SignalName.DialogueStarted, entity);
	}

	public void CanStartDialogue(bool value)
	{
		EmitSignal(SignalName.CanDialogue, value);
    }

}
