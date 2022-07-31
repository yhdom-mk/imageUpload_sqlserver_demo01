using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageUpload_sqlserver_demo01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: このコード行はデータを 'database1DataSet.Pictures' テーブルに読み込みます。必要に応じて移動、または削除をしてください。
            //this.picturesTableAdapter.Fill(this.database1DataSet.Pictures);
            LoadData();

        }

        public void Insert(string fileName, byte[] image)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings[
                "ImageUpload_sqlserver_demo01.Properties.Settings.Database1ConnectionString"].ConnectionString))
            {
                if(cn.State == ConnectionState.Closed)
                    cn.Open();
                using(SqlCommand cmd = SqlCommand("insert into pictures(filename, image) values(@filename, @image)"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@filename", textFileName.Text);
                    cmd.Parameters.AddWithValue("@image", image);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LoadData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings[
                "ImageUpload_sqlserver_demo01.Properties.Settings.Database1ConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                using(DataTable dt = new DataTable("pictures"))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("select *from pictures", cn);
                    adapter.Fill(dt);
                    dataGridView.DataSource = dt;
                }
            }
        }

        byte[] ConvertImageToBytes(Image img)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public Image ConvertByteArrayToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog() { Filter="Image files(*.jpg;*.jpeg)|*.jpg;*.jpeg", Multiselect=false })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    //display image to pictureBox
                    pictureBox.Image = Image.FromFile(ofd.FileName);
                    //set path
                    textFileName.Text = ofd.FileName;
                    //insert data to localdatabase, then reload data
                    Insert(textFileName.Text, ConvertImageToBytes(pictureBox.Image));
                    LoadData();
                }
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dt = dataGridView.DataSource as DataTable;
            if(dt != null)
            {
                DataRow row = dt.Rows[e.RowIndex];
                pictureBox.Image = ConvertByteArrayToImage((byte[])row["Image"]);
            }
        }
    }
}
