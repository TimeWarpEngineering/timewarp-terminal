namespace TimeWarp.Terminal.Layout;

#region Purpose
// Fluent builder for flexbox layouts of terminal widgets and text.
#endregion

/// <summary>
/// Fluent builder for composing <see cref="Layout"/> trees from panels, tables, rules, and text.
/// </summary>
public sealed class LayoutBuilder : IBuilder<Layout>
{
  private FlexDirection DirectionValue = FlexDirection.Column;
  private int GapValue;
  private TimeWarp.Flexbox.Wrap WrapValue = TimeWarp.Flexbox.Wrap.NoWrap;
  private Align AlignItemsValue = Align.Stretch;
  private Justify JustifyContentValue = Justify.FlexStart;
  private readonly List<LayoutChild> Children = [];

  /// <summary>
  /// Sets the flex direction for this container.
  /// </summary>
  /// <param name="direction">Row or column direction.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder Direction(FlexDirection direction)
  {
    DirectionValue = direction;
    return this;
  }

  /// <summary>
  /// Sets the gap between items in character cells.
  /// </summary>
  /// <param name="cells">Gap size in cells.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder Gap(int cells)
  {
    GapValue = cells;
    return this;
  }

  /// <summary>
  /// Sets flex-wrap behavior.
  /// </summary>
  /// <param name="wrap">Wrap mode.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder Wrap(TimeWarp.Flexbox.Wrap wrap)
  {
    WrapValue = wrap;
    return this;
  }

  /// <summary>
  /// Sets align-items for the cross axis.
  /// </summary>
  /// <param name="align">Alignment value.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder AlignItems(Align align)
  {
    AlignItemsValue = align;
    return this;
  }

  /// <summary>
  /// Sets justify-content for the main axis.
  /// </summary>
  /// <param name="justify">Justification value.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder JustifyContent(Justify justify)
  {
    JustifyContentValue = justify;
    return this;
  }

  /// <summary>
  /// Adds a panel item with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Action<PanelBuilder> panel)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(panel);
    PanelBuilder panelBuilder = new();
    panel(panelBuilder);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Panel, panel: panelBuilder.Build());
  }

  /// <summary>
  /// Adds a table item with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Action<TableBuilder> table)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(table);
    TableBuilder tableBuilder = new();
    table(tableBuilder);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Table, table: tableBuilder.Build());
  }

  /// <summary>
  /// Adds a rule item with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Action<RuleBuilder> rule)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(rule);
    RuleBuilder ruleBuilder = new();
    rule(ruleBuilder);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Rule, rule: ruleBuilder.Build());
  }

  /// <summary>
  /// Adds a text item with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, string text)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(text);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Text, text: text);
  }

  /// <summary>
  /// Adds a pre-built panel with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Panel panel)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(panel);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Panel, panel: panel);
  }

  /// <summary>
  /// Adds a pre-built table with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Table table)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(table);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Table, table: table);
  }

  /// <summary>
  /// Adds a pre-built rule with flex options.
  /// </summary>
  public LayoutBuilder Item(Action<FlexItemBuilder> flex, Rule rule)
  {
    ArgumentNullException.ThrowIfNull(flex);
    ArgumentNullException.ThrowIfNull(rule);
    return AddLeaf(ConfigureFlex(flex), LayoutLeafKind.Rule, rule: rule);
  }

  /// <summary>
  /// Adds a panel item with default flex options.
  /// </summary>
  public LayoutBuilder Item(Action<PanelBuilder> panel)
  {
    ArgumentNullException.ThrowIfNull(panel);
    PanelBuilder panelBuilder = new();
    panel(panelBuilder);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Panel, panel: panelBuilder.Build());
  }

  /// <summary>
  /// Adds a table item with default flex options.
  /// </summary>
  public LayoutBuilder Item(Action<TableBuilder> table)
  {
    ArgumentNullException.ThrowIfNull(table);
    TableBuilder tableBuilder = new();
    table(tableBuilder);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Table, table: tableBuilder.Build());
  }

  /// <summary>
  /// Adds a rule item with default flex options.
  /// </summary>
  public LayoutBuilder Item(Action<RuleBuilder> rule)
  {
    ArgumentNullException.ThrowIfNull(rule);
    RuleBuilder ruleBuilder = new();
    rule(ruleBuilder);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Rule, rule: ruleBuilder.Build());
  }

  /// <summary>
  /// Adds a text item with default flex options.
  /// </summary>
  public LayoutBuilder Item(string text)
  {
    ArgumentNullException.ThrowIfNull(text);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Text, text: text);
  }

  /// <summary>
  /// Adds a pre-built panel with default flex options.
  /// </summary>
  public LayoutBuilder Item(Panel panel)
  {
    ArgumentNullException.ThrowIfNull(panel);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Panel, panel: panel);
  }

  /// <summary>
  /// Adds a pre-built table with default flex options.
  /// </summary>
  public LayoutBuilder Item(Table table)
  {
    ArgumentNullException.ThrowIfNull(table);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Table, table: table);
  }

  /// <summary>
  /// Adds a pre-built rule with default flex options.
  /// </summary>
  public LayoutBuilder Item(Rule rule)
  {
    ArgumentNullException.ThrowIfNull(rule);
    return AddLeaf(new FlexItemOptions(), LayoutLeafKind.Rule, rule: rule);
  }

  /// <summary>
  /// Adds a nested row container (direction set to <see cref="FlexDirection.Row"/>).
  /// </summary>
  /// <param name="configure">Configures the nested row builder.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder Row(Action<LayoutBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);
    LayoutBuilder nested = new();
    nested.DirectionValue = FlexDirection.Row;
    configure(nested);
    Children.Add(new LayoutChild
    {
      Flex = new FlexItemOptions(),
      Nested = nested.ToContainer()
    });
    return this;
  }

  /// <summary>
  /// Adds a nested column container (direction set to <see cref="FlexDirection.Column"/>).
  /// </summary>
  /// <param name="configure">Configures the nested column builder.</param>
  /// <returns>This builder for chaining.</returns>
  public LayoutBuilder Column(Action<LayoutBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);
    LayoutBuilder nested = new();
    nested.DirectionValue = FlexDirection.Column;
    configure(nested);
    Children.Add(new LayoutChild
    {
      Flex = new FlexItemOptions(),
      Nested = nested.ToContainer()
    });
    return this;
  }

  /// <summary>
  /// Builds the configured <see cref="Layout"/>.
  /// </summary>
  public Layout Build() => new(ToContainer());

  /// <summary>
  /// Builds the configured <see cref="Layout"/> (alias of <see cref="Build"/>).
  /// </summary>
  public Layout ToLayout() => Build();

  private LayoutBuilder AddLeaf(
    FlexItemOptions flex,
    LayoutLeafKind kind,
    string? text = null,
    Panel? panel = null,
    Table? table = null,
    Rule? rule = null)
  {
    Children.Add(new LayoutChild
    {
      Flex = flex,
      Leaf = new LayoutLeaf
      {
        Kind = kind,
        Text = text,
        Panel = panel,
        Table = table,
        Rule = rule
      }
    });
    return this;
  }

  private static FlexItemOptions ConfigureFlex(Action<FlexItemBuilder> flex)
  {
    FlexItemBuilder builder = new();
    flex(builder);
    return builder.ToOptions();
  }

  private LayoutContainer ToContainer() => new()
  {
    Direction = DirectionValue,
    Gap = GapValue,
    Wrap = WrapValue,
    AlignItems = AlignItemsValue,
    JustifyContent = JustifyContentValue,
    Children = [.. Children]
  };
}
