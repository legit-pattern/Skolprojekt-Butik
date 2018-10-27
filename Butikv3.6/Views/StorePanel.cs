﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Butikv3._6
{
    class Product
    {
        public int price;
        public string name;
        public string type;
        public string summary;
        public string imageLocation;
        public int nrOfProducts;

        private void Product_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.summary);
        }

        public Product GetProduct()
        {
            return this;
        }

        public string ToCSV()
        {
            return $"{price},{name},{type},{summary},{imageLocation},{nrOfProducts}";
        }
    }

    class StorePanel : TableLayoutPanel
    {
        #region properties used in storePanel and all functions
        // CartPanel 
        CartPanel cartPanelRef;

        TableLayoutPanel leftPanel;
        TableLayoutPanel searchControlerPanel;
        Button searchButton;
        TextBox searchBox;
        Button typeButton;
        TableLayoutPanel typePanel;
        //TableLayoutPanel storePanel;
        TableLayoutPanel productPanel;

        // Controls connected to description panel
        PictureBox descriptionPicture;
        Label descriptionNameLabel;
        Label descriptionSummaryLabel;
        TableLayoutPanel descriptionPanel;

        //This label is used in productPanel and descriptionPanel.
        Label nameLabel;
        Label priceLabel;
        PictureBox pictureBox;
        Button addToCartButton;
        // The four controls listed above is in itemPanel when it's added to storePanel,
        // in function PopulateStore.
        FlowLayoutPanel itemPanel;
        #endregion

        List<Product> productList = new List<Product>();
        List<string> typeList = new List<string>();

        public StorePanel(CartPanel reference)
        {
            cartPanelRef = reference;

            // Implement event that autoscales the MyForm window
            // so that images retain their scale.
            #region Main panel, 2 columns, refered to as "this.".
            this.ColumnCount = 2;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Transparent;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82));
            #endregion

            #region Left side table of "this.".
            leftPanel = new TableLayoutPanel
            {
                RowCount = 3,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
            };
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            this.Controls.Add(leftPanel);

            searchControlerPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Height = 15,
                Width = 55,
            };
            searchControlerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85));
            searchControlerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25));
            leftPanel.Controls.Add(searchControlerPanel);

            searchBox = new TextBox
            {
                Anchor = AnchorStyles.Top,
                Margin = new Padding(-20,0,-10,0),
                Width = 200,
            };
            searchControlerPanel.Controls.Add(searchBox);
            searchBox.KeyDown += new KeyEventHandler(SearchBox_Enter);
            searchButton = new Button
            {
                BackgroundImage = Image.FromFile(@"Icons/searchButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                Dock = DockStyle.Fill,
                Margin = new Padding(0,0,0,7),
                Height = 25,
            };
            searchButton.Click += SearchButton_Click;
            searchControlerPanel.Controls.Add(searchButton);

            typePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Width = 130,
                Padding = new Padding(0,0,0,0),
            };

            // Only to create a small space between filterbox and typebuttons.
            Label l = new Label
            {
                Dock = DockStyle.Fill,
            };
            leftPanel.Controls.Add(l);
            leftPanel.Controls.Add(typePanel);
            #endregion

            #region Right side table of this, holds itemPanel (menu with products).
            TableLayoutPanel rightPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
            };
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 60));
            this.Controls.Add(rightPanel);

            itemPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.Fixed3D,
                Margin = new Padding(0),
            };
            rightPanel.Controls.Add(itemPanel);
            #endregion

            #region Panel with controls, nested inside rightPanel

            descriptionPanel = new TableLayoutPanel
            {
                RowCount = 3,
                Dock = DockStyle.Fill,
            };
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            descriptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            rightPanel.Controls.Add(descriptionPanel);

            descriptionPicture = new PictureBox
            {
                BorderStyle = BorderStyle.Fixed3D,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromFile("Icons/placeholder.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
            };
            descriptionPanel.Controls.Add(descriptionPicture);

            descriptionNameLabel = new Label
            {
                Text = "Items name",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
            };
            descriptionPanel.Controls.Add(descriptionNameLabel);

            descriptionSummaryLabel = new Label
            {
                Text = "Items summary",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                FlatStyle = FlatStyle.Popup,
            };
            descriptionPanel.Controls.Add(descriptionSummaryLabel);
            #endregion

            QueryFromCSVToList();
            PopulateTypePanel(typeList);
            PopulateStore(productList);
        }

        // Method to display picture, name and summary of item in storePanel.
        private void UpdateProductView(Product tag)
        {
            descriptionPicture.ImageLocation = tag.imageLocation;
            descriptionNameLabel.Text = tag.name;
            descriptionSummaryLabel.Text = tag.summary;
        }

        // On button click inside storePanel.
        private void SearchButton_Click(object sender, EventArgs e)
        {
            if(searchBox.Text == string.Empty)
            {
                itemPanel.Controls.Clear();
                PopulateStore(productList);
            }
            else
            {
                itemPanel.Controls.Clear();
                PopulateStoreByFilter(productList, searchBox.Text);
            }
        }
        private void SearchBox_Enter(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(searchBox.Text == "")
                {
                    itemPanel.Controls.Clear();
                    PopulateStore(productList);
                }
                else
                {
                    itemPanel.Controls.Clear();
                    PopulateStoreByFilter(productList, searchBox.Text);
                }
            }
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
        private void TypeButton_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            searchButton.Focus();
            itemPanel.Controls.Clear();
            PopulateStoreByType(productList, b.Tag.ToString());
        }
        private void AddToCartButton_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            cartPanelRef.AddToCart((Product)b.Tag);
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(TableLayoutPanel))
            {
                TableLayoutPanel t = (TableLayoutPanel)sender;
                UpdateProductView((Product)t.Tag);
            }
            else if(sender.GetType() == typeof(PictureBox))
            {
                PictureBox p = (PictureBox)sender;
                UpdateProductView((Product)p.Tag);
            }
            else if(sender.GetType() == typeof(Label))
            {
                Label l = (Label)sender;
                UpdateProductView((Product)l.Tag);
            }
        }

        // Methods that populate storePanel/typePanel.
        private void PopulateStore(List<Product> productList)
        {
            foreach (Product item in productList)
            {
                productPanel = new TableLayoutPanel
                {
                    ColumnCount = 4,
                    RowCount = 1,
                    Anchor = AnchorStyles.Top,
                    Height = 60,
                    Width = 400,
                    Margin = new Padding(0),
                };
                productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
                productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                productPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                itemPanel.Controls.Add(productPanel);

                pictureBox = new PictureBox
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Dock = DockStyle.Top,
                    Image = Image.FromFile(item.imageLocation),
                };
                productPanel.Controls.Add(pictureBox);

                nameLabel = new Label
                {
                    Text = item.name,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Anchor = AnchorStyles.Left,
                };
                productPanel.Controls.Add(nameLabel);

                priceLabel = new Label
                {
                    Text = item.price + "kr",
                    TextAlign = ContentAlignment.MiddleLeft,
                    Anchor = AnchorStyles.Left,
                };
                productPanel.Controls.Add(priceLabel);

                addToCartButton = new Button
                {
                    Text = "Lägg i kundvagn",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Popup,
                    BackColor = Color.DarkKhaki,
                    Dock = DockStyle.Fill,
                };
                productPanel.Controls.Add(addToCartButton);
                pictureBox.Click += PictureBox_Click;
                pictureBox.Tag = item;

                productPanel.Click += PictureBox_Click;
                nameLabel.Click += PictureBox_Click;
                priceLabel.Click += PictureBox_Click;
                addToCartButton.Click += AddToCartButton_Click;

                productPanel.Tag = item;
                nameLabel.Tag = item;
                priceLabel.Tag = item;
                addToCartButton.Tag = item;
            }
        }
        private void PopulateTypePanel(List<string> typeList)
        {
            foreach (var item in typeList)
            {
                typeButton = new Button
                {
                    Text = item,
                    FlatStyle = FlatStyle.Popup,
                    BackColor = Color.DarkKhaki,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 30,
                    Width = 100,
                    Margin = new Padding(0,0,32,0),
                };
                typePanel.Controls.Add(typeButton);
                typeButton.Click += TypeButton_Click;
                typeButton.Tag = item;
            }
        }
        private void PopulateStoreByType(List<Product> productList, string type)
        {
            foreach (var item in productList)
            {
                if (type == item.type)
                {
                    productPanel = new TableLayoutPanel
                    {
                        ColumnCount = 4,
                        RowCount = 1,
                        Anchor = AnchorStyles.Top,
                        Height = 60,
                        Width = 400,
                        Margin = new Padding(0),
                    };
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    itemPanel.Controls.Add(productPanel);

                    pictureBox = new PictureBox
                    {
                        BorderStyle = BorderStyle.Fixed3D,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Dock = DockStyle.Top,
                        Image = Image.FromFile(item.imageLocation),
                    };
                    productPanel.Controls.Add(pictureBox);

                    nameLabel = new Label
                    {
                        Text = item.name,
                        TextAlign = ContentAlignment.MiddleLeft,
                    };
                    productPanel.Controls.Add(nameLabel);

                    priceLabel = new Label
                    {
                        Text = item.price + "kr",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                    };
                    productPanel.Controls.Add(priceLabel);

                    addToCartButton = new Button
                    {
                        Text = "Lägg i kundvagn",
                        TextAlign = ContentAlignment.MiddleCenter,
                        FlatStyle = FlatStyle.Popup,
                        BackColor = Color.DarkKhaki,
                        Dock = DockStyle.Fill,
                    };
                    productPanel.Controls.Add(addToCartButton);
                    pictureBox.Click += PictureBox_Click;
                    pictureBox.Tag = item;

                    productPanel.Click += PictureBox_Click;
                    nameLabel.Click += PictureBox_Click;
                    priceLabel.Click += PictureBox_Click;
                    addToCartButton.Click += AddToCartButton_Click;

                    productPanel.Tag = item;
                    nameLabel.Tag = item;
                    priceLabel.Tag = item;
                    addToCartButton.Tag = item;
                }
            }
        }
        private void PopulateStoreByFilter(List<Product> productList, string text)
        {
            foreach (Product item in productList)
            {
                if(item.name == text || item.type == text)
                {
                    productPanel = new TableLayoutPanel
                    {
                        ColumnCount = 4,
                        RowCount = 1,
                        Anchor = AnchorStyles.Top,
                        Height = 60,
                        Width = 400,
                        Margin = new Padding(0),
                    };
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                    productPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    itemPanel.Controls.Add(productPanel);

                    pictureBox = new PictureBox
                    {
                        BorderStyle = BorderStyle.Fixed3D,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Dock = DockStyle.Top,
                        Image = Image.FromFile(item.imageLocation),
                    };
                    productPanel.Controls.Add(pictureBox);

                    nameLabel = new Label
                    {
                        Text = item.name,
                        TextAlign = ContentAlignment.MiddleLeft,
                    };
                    productPanel.Controls.Add(nameLabel);

                    priceLabel = new Label
                    {
                        Text = item.price + "kr",
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                    };
                    productPanel.Controls.Add(priceLabel);

                    addToCartButton = new Button
                    {
                        Text = "Lägg i kundvagn",
                        TextAlign = ContentAlignment.MiddleCenter,
                        FlatStyle = FlatStyle.Popup,
                        BackColor = Color.DarkKhaki,
                        Dock = DockStyle.Fill,
                    };
                    productPanel.Controls.Add(addToCartButton);
                    pictureBox.Click += PictureBox_Click;
                    pictureBox.Tag = item;

                    productPanel.Click += PictureBox_Click;
                    nameLabel.Click += PictureBox_Click;
                    priceLabel.Click += PictureBox_Click;
                    addToCartButton.Click += AddToCartButton_Click;

                    productPanel.Tag = item;
                    nameLabel.Tag = item;
                    priceLabel.Tag = item;
                    addToCartButton.Tag = item;
                }
            }
        }

        // Collect data from csv and store in storeList.
        // also store in typeList, methods that filter.
        private void QueryFromCSVToList()
        {
            string[][] path = File.ReadAllLines(@"TextFile1.csv").Select(x => x.Split(',')).
                Where(x => x[0] != "" && x[1] != "" && x[2] != "" && x[3] != "" && x[4] != "").
                ToArray();

            for (int i = 0; i < path.Length; i++)
            {
                if (!typeList.Contains(path[i][2]))
                {
                    typeList.Add(path[i][2]);
                }
                Product tmp = new Product
                {
                    price = int.Parse(path[i][0]),
                    name = path[i][1],
                    type = path[i][2],
                    summary = path[i][3],
                    imageLocation = path[i][4],
                    nrOfProducts = 1,
                };
                productList.Add(tmp);
            }
            productList = productList.OrderBy(x => x.type).ToList();
            typeList = typeList.OrderBy(x => x).ToList();
        }
    }
}
