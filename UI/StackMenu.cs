using DataLayer.Controller;

namespace UI;

public class StackMenu : IMenu
{
    private readonly CardController _card;
    private readonly StackController _stack;

    public StackMenu(CardController card, StackController stack)
    {
        _card = card;
        _stack = stack;
    }
    
    public CardController CardTable { get => _card; init  => _card = value; }
    public StackController StackTable { get => _stack; init   => _stack = value; }
    
    public void DisplayMenu()
    {
        throw new NotImplementedException();
    }
}