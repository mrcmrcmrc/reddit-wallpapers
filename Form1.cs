using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace wallpapers
{
    public partial class Form1 : Form
    {
        List<string> images;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            images = new List<string>();

        }


        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
            Execute();
        }

        private void Execute()
        {
            string lastPostID = "";

            /* Uri url = new Uri("http://www.google.com");
           WebClient client = new WebClient();
           string html = client.DownloadString(url);
           //File.WriteAllText("mytext.txt", html);// 
           */

            HtmlWeb hw = new HtmlWeb();

            HtmlAgilityPack.HtmlDocument doc = null;
            Console.WriteLine(hw.StatusCode);
            for (int p = 0; p < numericUpDown1.Value; p++)
            {
                if (p != 0)
                    lastPostID = getLastPostID(doc);
                string myURL = createQueryString(p, lastPostID);
                Console.WriteLine(myURL);
                doc = hw.Load(myURL);
                images.AddRange(getImageLinks(doc));

                //doc = hw.Load(myURL);
                // // images.AddRange(GetImgurLinks(ExtractAllAHrefTags(doc)));
                //images.AddRange(getImageLinks(doc));
            }

            createUI();
        }


        private void Clear()
        {
            images.Clear();
           
            foreach (PictureBox pb in panel2.Controls)
            {
                Console.WriteLine("pb");
                pb.Dispose();
            }
          //  panel2.Dispose();
            panel2.Controls.Clear();
        }

        private string createQueryString(int p, string lastID)
        {
            string baseuri = "https://www.reddit.com/r/";
            string t = "";
            string nextPage = "?count=" + 25 * p + "&after=" + lastID;
            string subreddit = comboBox1.Text;

            if (comboBox1.Text == "sfwpornnetwork")
            {
                baseuri = "https://www.reddit.com/user/";
                subreddit = "kjoneslol/m/sfwpornnetwork";
            }
            if (radioButton7.Checked)
            {
                if (radioButton1.Checked)
                    t = "/top/?sort=top&t=day";
                else if (radioButton2.Checked)
                    t = "/top/?sort=top&t=week";
                else if (radioButton3.Checked)
                    t = "/top/?sort=top&t=month";
                else if (radioButton4.Checked)
                    t = "/top/?sort=top&t=year";
                else if (radioButton5.Checked)
                    t = "/top/?sort=top&t=all";
                else if (radioButton6.Checked)
                    t = "/top/?sort=top&t=hour";

                if (p == 0)
                    return  baseuri + subreddit + t;
                else
                {
                    nextPage = "&count=" + 25 * p + "&after=" + lastID; //use &count instead of ?count for filter by time.
                    return baseuri + subreddit + t + nextPage;
                }
                    
                }

            else if(radioButton8.Checked)
            {
                if (p == 0)
                    return baseuri + subreddit + "/hot";
                else
                    return baseuri + subreddit + "/hot" + nextPage;
            }

            else if(radioButton9.Checked)
            {
                if (p == 0)
                    return baseuri + subreddit + "/new";
                else
                    return baseuri + subreddit + "/new" + nextPage;
            }

            return null;
              
        }

        private string getLastPostID(HtmlAgilityPack.HtmlDocument _doc) 
        {
            var allElementsWithClassFloat = _doc.DocumentNode.SelectNodes("//*[contains(@class,'thing')]");

            return "t3_" + (allElementsWithClassFloat[allElementsWithClassFloat.Count-1].Id.Split('_'))[2];

        } // need lastPostID for next page

        private List<string> ExtractAllAHrefTags(HtmlAgilityPack.HtmlDocument htmlSnippet)
        {
            List<string> hrefTags = new List<string>();

            foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                hrefTags.Add(att.Value);
            }

            return hrefTags;
        }// ilk kullandığım yöntem. bütün href leri alıyor.

        private List<string> GetImgurLinks(List<string> links)
        {
            List<string> l = new List<string>();
            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].Contains("domain"))
                    continue;
                if (links[i].Contains("reddituploads"))
                {
                    links[i] = convertURLreddit(links[i]);
                    l.Add(links[i]);
                    i++;
                    continue;
                }
                if (links[i].Contains("imgur"))
                {
                    if (!(links[i].Contains(".jpg") || links[i].Contains(".png")))
                        links[i] = convertURL(links[i]);
                    l.Add(links[i]);
                    i++;               
                }

            }
            return l;
        }//old

        private string convertURL(string s)
        {
            string[] st = new string[10];
            foreach (string x in s.Split('/'))
                st[2] = x;

            return s = "https://i.imgur.com/" + st[2] + ".jpg";
        }//old

        private List<string> getImageLinks(HtmlAgilityPack.HtmlDocument htmldoc)
        {
            List<string> links = new List<string>();

            foreach (HtmlNode divnode in htmldoc.DocumentNode.SelectNodes("//div[@data-url]"))
            {
                HtmlAttribute att = divnode.Attributes["data-url"];
                if (att.Value.Contains("comments"))
                    continue;
                if (att.Value.Contains("reddituploads"))
                {
                    links.Add(convertURLreddit(att.Value));
                    continue;
                }
                if (!att.Value.Contains(".jpg") && !att.Value.Contains(".png"))
                    continue;
                links.Add(att.Value);
               
            }

            return links;
        } // 

        private string convertURLreddit(string s) 
        {
            string[] temp = s.Split('?');
            string[] str = temp[1].Split(new string[] { "amp;" }, StringSplitOptions.None);
            string n = "";
            foreach (string i in str)
                n += i;
            return temp[0] + "?" + n;
        } // reddituploads.com
        private void createUI()
        {

            int x = 0;
            int y = 0;
            int marginY = 10;
            int marginX = 10;
            PictureBox[] pb = new PictureBox[images.Count];

            for (int i = 0; i < images.Count; i++)
            {
               
                pb[i] = new PictureBox();
                pb[i].Location = new Point(5 + x, y);
                pb[i].Size = new Size(250, 210);
                pb[i].SizeMode = PictureBoxSizeMode.StretchImage;
            
                pb[i].ImageLocation = images[i];
                pb[i].Name = "pb" + i.ToString();
                pb[i].BorderStyle = BorderStyle.FixedSingle;
                pb[i].MouseClick += SaveAs_Click;
                //this.Controls.Add(pb[i]);
                panel2.Controls.Add(pb[i]);
              
                x += pb[i].Size.Width + marginX;
                if(x+pb[i].Size.Width >= panel2.Size.Width)
                {
                    x = 0;
                    y += pb[i].Size.Height + marginY;
                }
                label1.Text = i.ToString();        
            }

            label1.Text = images.Count + "....";
        }

        void SaveAs_Click(object sender, MouseEventArgs e)
        {
            PictureBox pbox = sender as PictureBox;

            // handle click event
            if (e.Button == MouseButtons.Left)
            {
                pbox.Image.Save(pbox.Name + ".jpg");
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                groupBox2.Enabled = true;
            }
            else
                groupBox2.Enabled = false;
        }
    }

}
