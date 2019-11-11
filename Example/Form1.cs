using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Example
{
    public partial class Form1 : Form
    {
        Socket socket;
        Screan sc = new Screan();
        Boolean joinroom = false;
        string userroomnum;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            sc.Show();
            listView1.View = View.Details;
            //using System.Windows.Forms;

            //コンボボックスにディスプレイのリストを表示する
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            //デバイス名が表示されるようにする
            this.comboBox1.DisplayMember = "DeviceName";
            this.comboBox1.DataSource = Screen.AllScreens;

            //Button1のClickイベントハンドラ
        }

        private void RecvNewMessage(JObject jobject)
        {

            var message = jobject.ToObject<Msgcreat>();
            Byte[] tb = System.Text.Encoding.Default.GetBytes(txtMessage.Text);
            string str = Encoding.Unicode.GetString(message.Byt);
            message.Text = str;
            Console.WriteLine();
            this.Invoke((MethodInvoker)(() =>
            {

                string[] items = { message.User.ToString(), str, message.Time.ToString(), message.Id.ToString() };
                this.listView1.Items.Add(new ListViewItem(items));
                sc.Addtext(jobject);
            }));


        }
        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Sendmessage();
            }
        }
        private void Txtmsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            //EnterやEscapeキーでビープ音が鳴らないようにする
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }
        private void Sendmessage()
        {
            if (txtMessage.TextLength != 0 && socket != null)
            {

                var message = new NewMessage()
                {
                    User = "Teacher",
                    Text = this.txtMessage.Text,
                    Color = "#000",
                    Area = "ue",
                    Speed = "normal",
                    Size = "medium"
                };
                //Byte[] tb = System.Text.Encoding.Default.GetBytes(txtMessage.Text);
                //string str = System.Text.Encoding.Unicode.GetString(tb);
                //message.Text = str;
                var jobject = JObject.FromObject(message);
                txtMessage.Text = "";
                this.socket.Emit("msgcreat", jobject);
            }
        }
        private void BtnSend_Click(object sender, EventArgs e)
        {
            Sendmessage();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var random = new System.Random();
            var room = random.Next();
            textBox1.Text = room.ToString();
        }
        private void Addtextlabel(Msgcreat msg)
        {


        }
        private void LstMessages_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void TextBox1_KeyPress(object sender,
   System.Windows.Forms.KeyPressEventArgs e)
        {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {

            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && userroomnum != textBox1.Text)
            {
                if (joinroom)
                {
                    socket.Disconnect();
                }
                var room = textBox1.Text;
                //this.socket = IO.Socket("https://socketchat-dit.arkjp.net/");
                //this.socket = IO.Socket("http://nenohi.f5.si:3000");
                this.socket = IO.Socket("http://localhost:3000");

                this.socket.On(Socket.EVENT_CONNECT, () =>
                {


                    var joinuser = new Joinuser()
                    {
                        User = "Teacher",
                        Room = room.ToString()
                    };
                    userroomnum = room;
                    var jjoinuser = JObject.FromObject(joinuser);
                    this.socket.Emit("login_user", jjoinuser);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        string[] items = { "System", room + "に接続しました。", "", "" };
                        this.listView1.Items.Add(new ListViewItem(items));
                        foreach (ColumnHeader ch in listView1.Columns)
                        {
                            ch.Width = -1;
                        }
                    }));
                    joinroom = true;
                });

                this.socket.On("createdmsg", (jo) =>
                {

                    this.RecvNewMessage(jo as JObject);
                });
                this.socket.On("login_user", (userdata) =>
                {
                });
                this.socket.On("userlist", (ulist) =>
                {
                    
                    this.Addlistboxuser(ulist as JObject);
                });
            }
        }

        private void Addlistboxuser(JObject udata)
        {
            var userdata = udata.ToObject<Joinuser>();
            this.Invoke((MethodInvoker)(() =>
            {

                this.listBox1.Items.Add(userdata.User.ToString());
            }));
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //sc.Close();
            //表示させるフォームを作成する
            //フォームを表示するディスプレイのScreenを取得する
            Screen s = (Screen)this.comboBox1.SelectedItem;
            //フォームの開始位置をディスプレイの左上座標に設定する
            sc.WindowStartupLocation = WindowStartupLocation.Manual;
            sc.Height = s.Bounds.Height;
            sc.Width = s.Bounds.Width;
            sc.Top = s.Bounds.Location.Y;
            sc.Left = s.Bounds.Location.X;
            //フォームを表示する
            //sc.Show();
        }

        private void ComboBox1_CursorChanged(object sender, EventArgs e)
        {

        }
    }
}
