namespace Jelly.Experimental;

using Jelly;
using Jelly.Commands;
using Jelly.Library;
using Jelly.Values;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("cat", new SimpleCommand(CmdCat)); 

        // TODO:  inc
        // TODO:  dec
        // TODO:  //
        // TODO:  %
        // TODO:  %%
        // TODO:  **
        // TODO:  ~
        // TODO:  &
        // TODO:  |
        // TODO:  ^
        // TODO:  <
        // TODO:  <=
        // TODO:  = / ==
        // TODO:  != / <>
        // TODO:  >=
        // TODO:  >
        // TODO:  lt
        // TODO:  lte
        // TODO:  eq
        // TODO:  neq
        // TODO:  gt
        // TODO:  gte
        // TODO:  ilt
        // TODO:  ilte
        // TODO:  ieq
        // TODO:  ineq
        // TODO:  igt
        // TODO:  igte
        // TODO:  and / &&
        // TODO:  andthen
        // TODO:  or / ||
        // TODO:  orelse
        // TODO:  not / !
    }

    public Value CmdCat(IScope scope, ListValue args)
    {
        return string.Join("", args).ToValue();
    }
}
