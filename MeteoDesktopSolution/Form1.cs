using MeteoDesktopSolution.Data;
using MeteoDesktopSolution.db;
using MeteoDesktopSolution.Model;
using MongoDB.Bson;
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
        try
        {
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
        catch (Exception ex) {
            MessageBox.Show("Ha ocurriddo un error conseguir la lista de balizas , intentando cargar las balizas desde la base de datos");
            MongoController mongoController = MongoController.getMongoController();
            List<BsonDocument> stationList = mongoController.getStations();
            if (stationList.Count() == 0) {
                MessageBox.Show("No hay balizas que cargar en la base de datos");
            }
            DataTable stationDt = new DataTable("StationList");
            stationDt.Columns.Add("id");
            stationDt.Columns.Add("nombre");
            for (int i = 0; i < stationList.Count(); i++)
            {
                stationDt.Rows.Add(stationList[i].GetValue("_id"), stationList[i].GetValue("name"));
            }
            for (int j = 0; j < stationDt.Rows.Count; j++)
            {
                comboBox1.Items.Add(stationDt.Rows[j]["id"].ToString() + " " + stationDt.Rows[j]["nombre"].ToString());
            }
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
            catch (Exception ex) {
                MessageBox.Show("Ha ocurriddo un error conseguir datos de la baliza : " + stationId + "(" + stationName + ") intentando cargarla una desde la base de datos"  );
                try {
                    MongoController dbController = MongoController.getMongoController();
                    IDictionary<String, String> readingsMap = dbController.getLastReading(stationId);
                    foreach (var value in readingsMap)
                    {
                        Debug.WriteLine(value);
                    }
                    if (readingsMap.Count > 0)
                    {
                        DataGridViewRow newRow = new DataGridViewRow();
                        newRow.CreateCells(dataGridView1);
                        int loopCounter = 2;
                        newRow.Cells[0].Value = stationId;
                        newRow.Cells[1].Value = stationName;
                        foreach (KeyValuePair<String, String> cosa in readingsMap)
                        {
                            Debug.WriteLine(cosa);
                            newRow.Cells[loopCounter].Value = readingsMap[cosa.Key];
                            loopCounter++;
                        }
                        dataGridView1.Rows.Add(newRow);
                    }
                }
                catch (InvalidOperationException y)
                {
                    MessageBox.Show("No hay balizas en la base de datos");
                }
            } 
            
        }
    }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }
}