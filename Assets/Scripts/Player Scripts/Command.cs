using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public abstract void Execute(Animator anim);
}

public class MoveForwardCommand : Command
{
    public override void Execute(Animator anim)
    {
        anim.SetTrigger("Walk2");
    }
}

public class IdleCommand : Command
{
    public override void Execute(Animator anim)
    {
        anim.SetTrigger("Idle");
    }
}

public class AttackCommand : Command
{
    public override void Execute(Animator anim)
    {
        anim.SetTrigger("Atk2");
    }
}

public class TauntCommand : Command
{
    public override void Execute(Animator anim)
    {
        anim.SetTrigger("Taunt");
    }
}

public class EmptyCommand : Command
{
    public override void Execute(Animator anim)
    {
    }
}