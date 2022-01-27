using MeteoDesktopSolution.Data;
using MeteoDesktopSolution.db;
using MeteoDesktopSolution.Model;
using System.Collections.Generic;
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
        MongoController mongoController = new MongoController();
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

    private async void button1_Click(object sender, EventArgs e)
    {
        if (comboBox1.SelectedItem != null) {
            String stationId = this.comboBox1.SelectedItem.ToString().Split(" ")[0];
            String stationName = comboBox1.SelectedItem.ToString().Split(" ")[1];
            
            
            try
            {
                IDictionary<String, double> readingsMap = await DataParser.getStationData(stationId, stationName);
                Debug.WriteLine(readingsMap.Count());
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dataGridView1);
                int loopCounter = 2;
                newRow.Cells[0].Value = stationId;
                newRow.Cells[1].Value = stationName;
                foreach (KeyValuePair<String, double> cosa in readingsMap)
                {
                    Debug.WriteLine(cosa);
                    newRow.Cells[loopCounter].Value = readingsMap[cosa.Key];
                    loopCounter++;
                }
                dataGridView1.Rows.Add(newRow);
            }
            catch (Newtonsoft.Json.JsonReaderException ex) {
                MessageBox.Show("Ha ocurriddo un error conseguir datos de la baliza : " + stationId + "(" + stationName + ")"  );
            } 
            
        }
    }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }
}