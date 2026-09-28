using Calculatrice.Core;

namespace Calculatrice;

public partial class MainPage : ContentPage
{
    private readonly Calculator calculator = new();
    private bool? landscape;

    public MainPage() => InitializeComponent();

    private void OnKeyClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        calculator.Press(button.Text);
        ResultLabel.Text = calculator.IsError ? "Erreur" : calculator.Display;
        ExpressionLabel.Text = string.IsNullOrEmpty(calculator.Expression) ? " " : calculator.Expression;
        StatusLabel.Text = calculator.Message.Length > 0 ? calculator.Message
            : calculator.PendingOperation is not null ? "Saisissez le nombre suivant"
            : calculator.Expression.Length > 0 ? "Résultat" : "Prêt à calculer";
        ResultLabel.FontSize = calculator.Display.Length > 16 ? 24 : calculator.Display.Length > 10 ? 32 : 46;
        SemanticProperties.SetDescription(ResultLabel, $"Résultat : {ResultLabel.Text}");
        foreach (Button key in Keypad.Children.OfType<Button>())
        {
            bool selected = key.Text == calculator.PendingOperation;
            VisualStateManager.GoToState(key, selected ? "ActiveOperation" : "InactiveOperation");
            key.IsEnabled = !calculator.IsError || key.Text is "AC" or "⌫" or ","
                || (key.Text.Length == 1 && char.IsDigit(key.Text[0]));
        }
        if (button.Text == "=" || calculator.IsError)
            SemanticScreenReader.Default.Announce(calculator.IsError ? calculator.Message : $"Résultat : {calculator.Display}");
    }

    private void OnPageSizeChanged(object? sender, EventArgs e)
    {
        if (Body is null || Width <= 0 || Height <= 0) return;
        bool wide = Width > Height && Width >= 560;
        if (landscape != wide)
        {
            landscape = wide;
            Body.RowDefinitions.Clear();
            Body.ColumnDefinitions.Clear();
            Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            if (wide) Body.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.25, GridUnitType.Star)));
            else Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetRow(Keypad, wide ? 0 : 1);
            Grid.SetColumn(Keypad, wide ? 1 : 0);
        }
        // Preserve native touch targets; scrolling handles short viewports and large text.
        double available = Height - (wide ? 100 : 290);
        double keyHeight = Math.Clamp((available - 40) / 5, 48, 76);
        foreach (Button key in Keypad.Children.OfType<Button>()) key.HeightRequest = keyHeight;
        DisplayPanel.VerticalOptions = wide ? LayoutOptions.Center : LayoutOptions.Fill;
    }
}
