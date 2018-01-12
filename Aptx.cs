using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using Aptxcode.eMule;
using Aptxcode.BitTorrent;

namespace Aptxcode
{
    partial class Aptx : Form
    {
        string size;
        string ed2kl,filename;
        bool flag;
        //private string path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        private string path = Application.StartupPath;
        IntPtr handlePreview = IntPtr.Zero;
        private string recad = "[b]APTX字幕组招募：[/b]\r\n" +
            "翻译：日语一级，听力80分以上优先，有字幕制作经验优先，要求周日到周二能在线。\r\n" +
            "后期：有好的电脑和带宽，有一定电脑技术，会写avs优先，会使用x264命令行和megui。\r\n" +
            "时轴：有经验。\r\n" +
            "特效：会ass或AE。\r\n" +
            "海报：熟练运用制图软件，有一定创新能力，不拖稿。欲报名者请附上自己之前的作品\r\n";
        private string pm1 = "有意者请PM [url=http://bbs.aptx.cn/home.php?mod=spacecp&ac=pm&op=showmsg&handlekey=showmsg_112223&touid=112223&pmid=0&daterange=2&pid=7411897]xiaobin[/url]\r\n或者请发Email至aptx4869_subgroup@hotmail.com\r\n";
        private string pm2 = "有意者请至[url=bbs.aptx.cn]名侦探柯南事务所[/url]PM xiaobin\r\n或者请发Email至aptx4869_subgroup@hotmail.com\r\n";

        private readonly string[] trackers = new string[]{
            "http://t2.popgo.org:7456/annonce",
            "http://tracker.ktxp.com:6868/announce",
            "http://tracker.ktxp.com:7070/announce",
            "http://tracker.openbittorrent.com:80/announce",
            "http://tracker.publicbt.com:80/announce",
            "http://tracker.prq.to/announce",
            "http://www.sakuraflying.cn:2710/announce",
            "http://share.camoe.cn:8080/announce",
            "http://tracker.lbs123456.com:233/announce"
        };

       #region 程序集属性访问器

        public string AssemblyTitle
        {
            get
            {
                // 获取此程序集上的所有 Title 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // 如果至少有一个 Title 属性
                if (attributes.Length > 0)
                {
                    // 请选择第一个属性
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // 如果该属性为非空字符串，则将其返回
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                        return titleAttribute.Title;
                }
                // 如果没有 Title 属性，或者 Title 属性为一个空字符串，则返回 .exe 的名称
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                // 获取此程序集的所有 Description 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // 如果 Description 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Description 属性，则返回该属性的值
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                // 获取此程序集上的所有 Product 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // 如果 Product 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Product 属性，则返回该属性的值
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                // 获取此程序集上的所有 Copyright 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // 如果 Copyright 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Copyright 属性，则返回该属性的值
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                // 获取此程序集上的所有 Company 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // 如果 Company 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Company 属性，则返回该属性的值
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        public string ApplicationPath
        {
            get { return path; }
        }

        public IntPtr HandlePreview
        {
            get { return handlePreview; }
            set { handlePreview = value; }
        }

        #endregion

        public Aptx()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void btn_build_Click(object sender, EventArgs e)
        {
            if (!checkinput())
                return;

            txt_bbstitle.Text = String.Format("[APTX4869][CONAN][{0}{1}][{2}]", (checkBox1.Checked ? "剧场版" + txt_vol.Text + " " : "第" + txt_vol.Text + "话 "), txt_name.Text, comboBox1.Text);
            /*
            txt_bbscode.Text = "名侦探柯南 第" + txt_vol.Text + "话 " + txt_name.Text +
                " " + comboBox1.Text + "<br /><br />制作组：APTX4869字幕组<br />发布组：APTX分流组<br />" +
                "RESEED有效期：" + DateTime.Today.AddMonths(1).AddDays(-1).ToString("yyyy年MM月dd日") +
                "<br />文件大小：" + size + "<br /><br />海报：<br /><br />[img]http://download.aptx.cn/bt/conan" +
                txt_vol.Text + ".jpg[/img]<br />[color=blue]<br />EM：\n<a href=\"" + ed2kl + "\">" + filename + "</a><br /><br />MO电信：[url=" +
                "http://my.mofile.com/aptx.cn]http://my.mofile.com/aptx.cn[/url]<br />MO网通：" +
                "[url=http://my.mofile.com/aptx4869.net]http://my.mofile.com/aptx4869.net[/url]<br /><br />" +
                "电信提取码：" + txt_mo1.Text + "<br />网通提取码：" + txt_mo2.Text + "<br />[/color]<br /><br />[color=red][b]" +
                "<br /><br /><br />在线看柯南![url]http://tv.aptx.cn[/url]<br />在线看" + txt_vol.Text +
                "事务所字幕版：[url]" + txt_tvol.Text + "[/url]<br /><br /><br /><br />[/b][/color]";
            txt_bbscode.Text = "名侦探柯南 第" + txt_vol.Text + "话 " + txt_name.Text +
                " " + comboBox1.Text + "\r\n\r\n制作组：APTX4869字幕组\r\n发布组：APTX分流组\r\n" +
                "RESEED有效期：" + DateTime.Today.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd") +
                "\r\n文件大小：" + size + "\r\n\r\n海报：\r\n\r\n[align=center][img]http://s473.photobucket.com/albums/rr98/APTXCN/conan" +
                txt_vol.Text + ".jpg[/img][/align]\r\n[color=blue]\r\n\r\n" + ed2kl + "\r\n\r\n" +
                "[align=center][table=100%][tr][td][align=center][color=blue][b]BT下载[/b][/color][/align][/td][/tr]" +
                "[tr][td][local]1[/local][/td][/tr][/table][/align]\r\n\r\nMO电信：[url]" + 
                "http://my.mofile.com/aptx.cn[/url]\r\nMO网通：" +
                "[url]http://my.mofile.com/aptx4869.net[/url]\r\n\r\n" +
                "电信提取码：" + txt_mo1.Text + "\r\n网通提取码：" + txt_mo2.Text + "\r\n[/color]\r\n\r\n[color=red][b]" +
                "\r\n\r\n\r\n在线看柯南![url]http://tv.aptx.cn[/url]\r\n";
            */
            txt_bbscode.Text = String.Format("名侦探柯南 {0}{1} {2}\r\n\r\n制作组：APTX4869字幕组\r\n发布组：APTX分流组\r\nRESEED有效期：{3:yyyy-MM-dd}\r\n文件大小：{4}\r\n\r\n{5}{6}\r\n海报：\r\n\r\n[align=center][img]http://s473.photobucket.com/albums/rr98/APTXCN/{7}{8}.jpg[/img][/align]\r\n[color=blue]\r\n\r\n\reMule:\n\r\n{9}{10}\r\n\r\n\r\n", (checkBox1.Checked ? "剧场版" + txt_vol.Text + " " : "第" + txt_vol.Text + "话 "), txt_name.Text, comboBox1.Text, DateTime.Today.AddMonths(1).AddDays(-1), size, recad, pm1, (checkBox1.Checked ? "m" : "conan"), txt_vol.Text, ed2kl, (string.IsNullOrEmpty(textBox1.Text)?"":"\r\n\r\nMagent:"+textBox1.Text));
            txt_bttitle.Text = String.Format("[APTX4869][CONAN][名侦探柯南 {0} {1}][HDTV][{2}]", (checkBox1.Checked ? "MOVIE" + txt_vol.Text : txt_vol.Text), txt_name.Text, comboBox1.Text);
            /*
            txt_btcode.Text = "名侦探柯南 第" + txt_vol.Text + "话 " + txt_name.Text +
                " " + comboBox1.Text + "\r\n\r\n制作组：APTX4869字幕组\r\n发布组：APTX分流组\r\n" +
                "RESEED有效期：" + DateTime.Today.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd") +
                "\r\n文件大小：" + size + "\r\n\r\n海报：\r\n\r\n[img]http://s473.photobucket.com/albums/rr98/APTXCN/conan" +
                txt_vol.Text + ".jpg[/img]\r\n\r\n\r\n[color=red][b]" +
                "\r\n\r\n\r\n在线看柯南![url]http://tv.aptx.cn[/url]\r\n";
            */
            txt_btcode.Text = String.Format("名侦探柯南 {0}{1} {2}\r\n\r\n制作组：APTX4869字幕组\r\n发布组：APTX分流组\r\nRESEED有效期：{3:yyyy-MM-dd}\r\n文件大小：{4}\r\n\r\n{5}{6}\r\n海报：\r\n\r\n[img]http://s473.photobucket.com/albums/rr98/APTXCN/{7}{8}.jpg[/img]", (checkBox1.Checked ? "剧场版" + txt_vol.Text + " " : "第" + txt_vol.Text + "话 "), txt_name.Text, comboBox1.Text, DateTime.Today.AddMonths(1).AddDays(-1), size, recad, pm2, (checkBox1.Checked ? "m" : "conan"), txt_vol.Text);
            /*
            if (txt_tvol.Text != "")
            {
                txt_bbscode.Text += "在线看" + txt_vol.Text + "事务所字幕版：[url]" +
                    txt_tvol.Text + "[/url]\r\n\r\n\r\n[/b][/color]";
                txt_btcode.Text += "在线看" + txt_vol.Text + "事务所字幕版：[url]" + 
                    txt_tvol.Text + "[/url]\r\n\r\n\r\n[/b][/color]";
            }
            else
            {
                txt_bbscode.Text += "在线看" + txt_vol.Text + "事务所字幕版：暂无\r\n\r\n\r\n[/b][/color]";
                txt_btcode.Text += "在线看" + txt_vol.Text + "事务所字幕版：暂无\r\n\r\n\r\n[/b][/color]";
            }
            */
            if (comboBox1.SelectedIndex == 1)
            {
                try
                {
                    txt_bbstitle.Text = CHConvert.ConvChsToCht(txt_bbstitle.Text);
                    txt_bbscode.Text = CHConvert.ConvChsToCht(txt_bbscode.Text);
                    txt_bttitle.Text = CHConvert.ConvChsToCht(txt_bttitle.Text);
                    txt_btcode.Text = CHConvert.ConvChsToCht(txt_btcode.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK);
                }
            }
            flag = true;            
        }

        private bool checkinput()
        {
            if (string.IsNullOrEmpty(txt_vol.Text))
            {
                MessageBox.Show("请输入第几" + (checkBox1.Checked ? "部" : "话"), "提示", MessageBoxButtons.OK);
                txt_vol.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_name.Text))
            {
                MessageBox.Show("请输入标题", "提示", MessageBoxButtons.OK);
                txt_name.Focus();
                return false;
            }
            /*
            if (txt_tvol.Text == "")
            {
                MessageBox.Show("请输入在线观看地址", "提示", MessageBoxButtons.OK);
                txt_tvol.Focus();
                return false;
            }
            */
            if (string.IsNullOrEmpty(txt_ed2k.Text))
            {
                MessageBox.Show("请输入ED2K链接", "提示", MessageBoxButtons.OK);
                txt_ed2k.Focus();
                return false;
            }

            ed2kl = txt_ed2k.Text.Trim();

            Regex ed2k = new Regex(@"ed2k://\|file\|(?<Name>.+)\|(?<Size>\d+)\|[0-9a-fA-F]+\|(/|h=\w+\|/)",RegexOptions.IgnoreCase);
            if (ed2k.IsMatch(ed2kl))
            {
                filename = ed2k.Match(ed2kl).Result("${Name}");
                Regex htn = new Regex(@"(?<HT>(%[0-9a-fA-F]{2})+)");
                MatchEvaluator me = decode;
                filename = htn.Replace(filename, me);
                //string h1 = ed2k.Match(txt_ed2k.Text.Trim()).Result("${H1}");
                //string h2 = ed2k.Match(txt_ed2k.Text.Trim()).Result("${H2}");
                double dsize = Convert.ToDouble(ed2k.Match(ed2kl).Result("${Size}"));
                if (dsize >= 1048576)
                {
                    double LenMB = dsize / 1048576.0;
                    size = LenMB.ToString("0.0") + " MB";
                }
                else if (dsize >= 1024)
                {
                    double LenKB = dsize / 1024.0;
                    size = LenKB.ToString("0.0") + " KB";
                }
                else
                    size = dsize.ToString() + "B"; 
            }
            else
            {
                MessageBox.Show("输入的ED2K链接不正确", "提示", MessageBoxButtons.OK);
                txt_ed2k.Focus();
                return false;
            }

            /*
            if (txt_mo1.Text == "")
                txt_mo1.Text = "暂无";

            if (txt_mo2.Text == "")
                txt_mo2.Text = "暂无";
            */ 

            return true;
        }
        static string decode(Match m)
        {
            try
            {
                string[] code = m.ToString().Split('%');
                byte[] bcode = new byte[code.Length - 1];
                for (int i = 0; i < bcode.Length; i++)
                {
                    bcode[i] = byte.Parse(code[i + 1], System.Globalization.NumberStyles.HexNumber);
                }
                return Encoding.UTF8.GetString(bcode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK);
                return String.Empty;
            }
        }

        private void btn_copy1_Click(object sender, EventArgs e)
        {
            Copy2Clipboard(txt_bbstitle.Text);
        }

        private void btn_copy2_Click(object sender, EventArgs e)
        {
            Copy2Clipboard(txt_bbscode.Text);
        }

        private void btn_copy4_Click(object sender, EventArgs e)
        {
            Copy2Clipboard(txt_bttitle.Text);
        }

        private void btn_copy3_Click(object sender, EventArgs e)
        {
            Copy2Clipboard(txt_btcode.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string postData = "subject=" + txt_bbstitle.Text;
            postData += ("&message=" + txt_bbscode.Text);
            //postData += ("&attach[]=" + );
            byte[] data = Encoding.ASCII.GetBytes(postData);
            HttpWebRequest post =
                (HttpWebRequest)WebRequest.Create("http://localhost/forum/post.php?action=newthread&fid=2&extra=page%3D1&topicsubmit=yes");
            post.Method = "POST";
            post.ContentType = "multipart/form-data";
            post.ContentLength = data.Length;
            post.CookieContainer = new CookieContainer();
            Stream postst = post.GetRequestStream();

            postst.Write(data, 0, data.Length);
            postst.Close();
            HttpWebResponse myResponse =
                (HttpWebResponse)post.GetResponse();
            using (StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8))
            {
                txt_btcode.Text = reader.ReadToEnd();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txt_vol.Clear();
            txt_name.Clear();
            //txt_mo1.Clear();
            //txt_mo2.Clear();
            //txt_tvol.Clear();
            txt_ed2k.Clear();
            txt_bbstitle.Clear();
            txt_bbscode.Clear();
            txt_bttitle.Clear();
            txt_btcode.Clear();
            flag = false;
            comboBox1.SelectedIndex = 0;
            checkBox1.Checked = false;
            /*
            string postData = "username=squall617&password=france&questionid=0&answer=&cookietime=315360000&loginmode=&styleid=1";
            byte[] data = Encoding.ASCII.GetBytes(postData);
            HttpWebRequest post =
                (HttpWebRequest)WebRequest.Create("http://localhost/forum/logging.php?action=login&");
            post.Method = "POST";
            //post.ContentType = "multipart/form-data";
            post.ContentLength = data.Length;
            post.CookieContainer = new CookieContainer();
            Stream postst = post.GetRequestStream();

            postst.Write(data, 0, data.Length);
            postst.Close();
            HttpWebResponse myResponse =
                (HttpWebResponse)post.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(),Encoding.UTF8);
            txt_btcode.Text = reader.ReadToEnd();
            */
        }

        private void Copy2Clipboard(string inputstr)
        {
            if (flag)
            {
                try
                {
                    Clipboard.SetText(inputstr);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            else
            {
                MessageBox.Show("您还没有生成代码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void 更新内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChangelog();            
        }
        
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void ShowChangelog()
        {
            string changelog = GetChangelog();

            ShowBox box = new ShowBox(changelog);
            box.ShowDialog();
        }

        private string GetChangelog()
        {
            string changelog = string.Empty;

            string logName = path + "\\Changelog.txt";

            changelog = File.Exists(logName) ? File.ReadAllText(logName) : "Lost Changelog.";

            return changelog;
        }

        private void ShowAbout()
        {
            string about = GetAbout();

            ShowBox box = new ShowBox(this, about, "关于 " + AssemblyTitle);
            box.ShowDialog();
        }

        private string GetAbout()
        {
            string about = "{0}\r\nVersion: {1}\r\n\r\n{2}\r\n{3}\r\n\r\nCore DLLs:\r\n";
            about = string.Format(about, AssemblyTitle, AssemblyVersion, AssemblyCompany, AssemblyCopyright);

            FileInfo[] dlls = new DirectoryInfo(path).GetFiles("*.dll");
            Array.ForEach(dlls, dll =>
            {
                try
                {
                    string company = ((AssemblyCompanyAttribute)Assembly.LoadFile(dll.FullName).GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0]).Company.Trim();
                    if (company.Equals(string.Empty))
                        company = "N/A";
                    about += string.Format("{0}: {1} ({2})\r\n", dll.Name, FileVersionInfo.GetVersionInfo(dll.FullName).FileVersion, company);
                }
                catch
                {
                    string company = FileVersionInfo.GetVersionInfo(dll.FullName).CompanyName.Trim();
                    if (company.Equals(string.Empty))
                        company = "N/A";
                    about += string.Format("{0}: {1} ({2})\r\n", dll.Name, FileVersionInfo.GetVersionInfo(dll.FullName).FileVersion, company);
                }
            });

            return about;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = checkBox1.Checked ? "第几部" : "第几话";
        }

        private void open_btn_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    txt_file.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            else
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                txt_file.Text = openFileDialog1.FileName;
            }
            btem_btn.Enabled = true;
        }

        private void btem_btn_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(txt_file.Text);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string file = e.Argument as string;
            Torrent torrent = new Torrent(trackers, file, null, string.Empty, false) { Worker = worker };
            byte[] buf1;
            string magnet;
            torrent.Create(out buf1, out magnet);
            ed2k ed2k = new ed2k { Worker = worker };
            string edlink = ed2k.GetLink(file);
            e.Result = new ArrayList { buf1, edlink, magnet };
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Torrent)
            {
                label3.Text = "正在生成 Torrent 种子文件...";
            }
            else if (e.UserState is ed2k)
            {
                label3.Text = "正在计算 ed2k 链接...";
            }
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ArrayList retlist = e.Result as ArrayList;
                byte[] bt1 = retlist[0] as byte[];
                //byte[] bt2 = retlist[1] as byte[];
                txt_ed2k.Text = retlist[1] as string;
                textBox1.Text = retlist[2] as string;
                try
                {
                    File.WriteAllBytes(Path.ChangeExtension(txt_file.Text, "torrent"), bt1);
                    //File.WriteAllBytes(Path.ChangeExtension(txt_file.Text, "gl.torrent"), bt2);
                    label3.Text = "已成功生成种子及 ed2k 链接";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void txt_ed2k_Click(object sender, EventArgs e)
        {
            txt_ed2k.SelectAll();
        }
    }
}