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

public partial class DialogSharpPlayer : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 300.0f;

    [Export]
    public DialogRayCast2d dialogRayCast2D { get; set; } = null!;

    public override void _Ready() {
        DialogUI.Instance.DialogStarted += DialogStarted;
		DialogUI.Instance.DialogFinished += DialogFinished;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("interact"))
            dialogRayCast2D.Interact();
    }

    public override void _PhysicsProcess(double delta)
    {


        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
            velocity.X -= 1;
        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
            velocity.X += 1;
        if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
            velocity.Y -= 1;
        if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
            velocity.Y += 1;

        velocity = velocity.Normalized();
        Velocity = velocity * Speed;

        if (velocity != Vector2.Zero)
        {
            Rotation = velocity.Angle();
        }

        MoveAndSlide();
    }

    private void DialogFinished()
    {
        SetPhysicsProcess(true);
        SetProcessInput(true);
    }

    private void DialogStarted()
    {
        SetPhysicsProcess(false);
        SetProcessInput(false);
    }
}
