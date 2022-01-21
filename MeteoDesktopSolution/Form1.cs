using MeteoDesktopSolution.Data;
using MeteoDesktopSolution.Model;
using System.Data;
using System.Diagnostics;

namespace MeteoDesktopSolution;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        Control.CheckForIllegalCrossThreadCalls = false;
        LoadComboBox();
    }

    private async void LoadComboBox() {
        Station[] stationList = (await DataParser.getStations().ConfigureAwait(false));
        DataTable stationDt = new DataTable("StationList");
        stationDt.Columns.Add("id");
        stationDt.Columns.Add("nombre");
        for (int i = 0; i < stationList.Length; i++)
        {
            stationDt.Rows.Add(stationList[i].id, stationList[i].name);
        }
        for (int j = 0; j < stationDt.Rows.Count; j++)
        {
            comboBox1.Items.Add(stationDt.Rows[j]["id"].ToString() + " " + stationDt.Rows[j]["nombre"].ToString());
        }

    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (comboBox1.SelectedItem != null) {
            String stationId = this.comboBox1.SelectedItem.ToString().Split(" ")[0];
            DataParser.getStationData(stationId);
        }
    }
}