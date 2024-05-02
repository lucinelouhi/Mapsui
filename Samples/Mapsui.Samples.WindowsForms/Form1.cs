using Mapsui.Extensions;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Extensions;
using Mapsui.UI.WindowsForms;

namespace Mapsui.Samples.WinForms;

public partial class Form1 : Form
{
    private readonly MapControl _mapControl;
    private readonly ComboBox _categoryComboBox;
    private readonly Panel _sampleList;
    private readonly TrackBar _rotationSlider;
    private readonly CheckedListBox _layerList;

    public Form1()
    {
        InitializeComponent();

        BackColor = Color.White;

        Mapsui.Tests.Common.Samples.Register();
        Mapsui.Samples.Common.Samples.Register();

        var layout = new TableLayoutPanel();

        layout.Dock = DockStyle.Fill;
        layout.ColumnCount = 2;
        layout.RowCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.25f));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.75f));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 1.0f));
        layout.BackColor = Color.White;

        _mapControl = new MapControl();

        _mapControl.Dock = DockStyle.Fill;
        _mapControl.AutoSize = true;

        layout.Controls.Add(_mapControl, 1, 0);

        var leftLayout = new TableLayoutPanel();

        leftLayout.Dock = DockStyle.Fill;
        leftLayout.ColumnCount = 1;
        leftLayout.RowCount = 4;
        leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f));
        leftLayout.RowStyles.Add(new ColumnStyle(SizeType.AutoSize));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 0.6f));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
        leftLayout.BackColor = Color.White;

        _categoryComboBox = new ComboBox();

        _categoryComboBox.Dock = DockStyle.Fill;
        _categoryComboBox.AutoSize = true;
        _categoryComboBox.SelectedValueChanged += CategoryChanged;

        leftLayout.Controls.Add(_categoryComboBox, 0, 0);

        _sampleList = new Panel();

        _sampleList.Dock = DockStyle.Fill;
        _sampleList.AutoSize = true;
        _sampleList.AutoScroll = true;

        leftLayout.Controls.Add(_sampleList, 0, 1);

        _rotationSlider = new TrackBar();

        _rotationSlider.Dock = DockStyle.Fill;
        _rotationSlider.AutoSize = true;
        _rotationSlider.Minimum = 0;
        _rotationSlider.Maximum = 360;
        _rotationSlider.Value = 0;
        _rotationSlider.ValueChanged += RotationChanged;

        _mapControl.Map.Navigator.ViewportChanged += ViewportChanged;

        leftLayout.Controls.Add(_rotationSlider, 0, 2);

        _layerList = new CheckedListBox();

        _layerList.Dock = DockStyle.Fill;
        _layerList.AutoSize = true;
        _layerList.ScrollAlwaysVisible = false;
        _layerList.CheckOnClick = true;
        _layerList.BackColor = Color.White;
        _layerList.BorderStyle = BorderStyle.None;

        _layerList.ItemCheck += ItemCheck;

        leftLayout.Controls.Add(_layerList, 0, 3);

        layout.Controls.Add(leftLayout, 0, 0);

        Controls.Add(layout);

        FillComboBoxWithCategories();
        FillListWithSamples();
    }

    private void ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        _mapControl.Map.Layers[e.Index].Enabled = e.NewValue == CheckState.Checked;
        _mapControl.Map.Refresh();
    }

    private void ViewportChanged(object sender, ViewportChangedEventArgs e)
    {
        if (_mapControl.Map.Navigator.Viewport.Rotation != _rotationSlider.Value)
            _rotationSlider.Value = (int)(_mapControl.Map.Navigator.Viewport.Rotation % 360);
    }

    private void RotationChanged(object? sender, EventArgs e)
    {
        if (_mapControl.Map.Navigator.Viewport.Rotation != _rotationSlider.Value)
            _mapControl.Map.Navigator.RotateTo(_rotationSlider.Value);
    }

    private void CategoryChanged(object? sender, EventArgs e)
    {
        FillListWithSamples();
    }

    private void FillComboBoxWithCategories()
    {
        Common.Samples.Register();
        Tests.Common.Samples.Register();

        var categories = AllSamples.GetSamples().Select(s => s.Category).Distinct().OrderBy(c => c).ToArray();

        _categoryComboBox.Items.AddRange(categories);

        _categoryComboBox.SelectedIndex = 2;
    }

    private void FillListWithSamples()
    {
        var selectedCategory = _categoryComboBox.SelectedItem?.ToString() ?? "";

        _sampleList.Controls.Clear();

        foreach (var sample in AllSamples.GetSamples().Where(s => s.Category == selectedCategory).Reverse())
        {
            _sampleList.Controls.Add(CreateRadioButton(sample));
        }

        ((RadioButton)_sampleList.Controls[_sampleList.Controls.Count - 1]).Checked = true;
    }

    private RadioButton CreateRadioButton(ISampleBase sample)
    {
        var radioButton = new RadioButton
        {
            Text = sample.Name,
            Dock = DockStyle.Top,
            BackColor = Color.White,
        };

        radioButton.CheckedChanged += (s, a) =>
        {
            if (s is RadioButton rb && !rb.Checked)
                return;

            Catch.Exceptions(async () =>
            {
                _mapControl.Map?.Layers.Clear();
                await sample.SetupAsync(_mapControl);
                _mapControl.Refresh();
                _layerList.Items.Clear();
                foreach (var layer in _mapControl.Map?.Layers)
                {
                    _layerList.Items.Add(layer.Name);
                    _layerList.SetItemCheckState(_layerList.Items.Count - 1, layer.Enabled ? CheckState.Checked : CheckState.Unchecked);
                }
            });
        };

        return radioButton;
    }
}
