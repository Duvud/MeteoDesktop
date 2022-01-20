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
    }

    private void button1_Click(object sender, EventArgs e)
    {
        
    }

    private async void button2_Click(object sender, EventArgs e)
    {
        Station[] stationList = (await DataParser.getStations().ConfigureAwait(false));
        DataTable stationDt = new DataTable("StationList");
        stationDt.Columns.Add("id");
        stationDt.Columns.Add("nombre");
        for (int i = 0; i < stationList.Length; i++) {
            stationDt.Rows.Add(stationList[i].id, stationList[i].name);
        }
        for (int j = 0; j < stationDt.Rows.Count; j++) {
            this.comboBox1.Items.Add(stationDt.Rows[j]["id"].ToString() + " " + stationDt.Rows[j]["nombre"].ToString());
        }
        /*
         *DataTable dt = new DataTable("Terrorists");
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            
            dt.Rows.Add("Osama", "Bin Laden");
            dt.Rows.Add("Saddam", "Hoessein");
            dt.Rows.Add("George", "Bush");

            for (int j = 0; j < dt.Rows.Count; j++)

            {
                string text = dt.Rows[j]["FirstName"].ToString() + " " + dt.Rows[j]["LastName"].ToString();
                this.comboBox1.Items.Add(text);
            }
         */
    }
}