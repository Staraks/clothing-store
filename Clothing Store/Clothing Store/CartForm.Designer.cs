namespace Clothing_Store
{
    partial class CartForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ListView listViewCart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnClearCart;
        private System.Windows.Forms.Button btnPlaceOrder;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox txtShippingAddress;
        private System.Windows.Forms.Label lblShippingAddress;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            listViewCart = new ListView();
            columnProductName = new ColumnHeader();
            columnSize = new ColumnHeader();
            columnQuantity = new ColumnHeader();
            columnPrice = new ColumnHeader();
            btnClose = new Button();
            btnClearCart = new Button();
            btnPlaceOrder = new Button();
            lblTotal = new Label();
            txtShippingAddress = new TextBox();
            lblShippingAddress = new Label();
            SuspendLayout();
            // 
            // listViewCart
            // 
            listViewCart.Columns.AddRange(new ColumnHeader[] { columnProductName, columnSize, columnQuantity, columnPrice });
            listViewCart.Location = new Point(32, 29);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(600, 300);
            listViewCart.TabIndex = 0;
            listViewCart.UseCompatibleStateImageBehavior = false;
            listViewCart.View = View.Details;
            // 
            // columnProductName
            // 
            columnProductName.Text = "Название товара";
            columnProductName.Width = 150;
            // 
            // columnSize
            // 
            columnSize.Text = "Размер";
            columnSize.Width = 100;
            // 
            // columnQuantity
            // 
            columnQuantity.Text = "Количество";
            columnQuantity.Width = 100;
            // 
            // columnPrice
            // 
            columnPrice.Text = "Цена";
            columnPrice.Width = 100;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(638, 29);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(94, 29);
            btnClose.TabIndex = 1;
            btnClose.Text = "Закрыть корзину";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnClearCart
            // 
            btnClearCart.Location = new Point(638, 64);
            btnClearCart.Name = "btnClearCart";
            btnClearCart.Size = new Size(94, 29);
            btnClearCart.TabIndex = 2;
            btnClearCart.Text = "Очистить корзину";
            btnClearCart.UseVisualStyleBackColor = true;
            btnClearCart.Click += btnClearCart_Click;
            // 
            // btnPlaceOrder
            // 
            btnPlaceOrder.Location = new Point(338, 400);
            btnPlaceOrder.Name = "btnPlaceOrder";
            btnPlaceOrder.Size = new Size(100, 29);
            btnPlaceOrder.TabIndex = 3;
            btnPlaceOrder.Text = "Оформить заказ";
            btnPlaceOrder.UseVisualStyleBackColor = true;
            btnPlaceOrder.Click += BtnPlaceOrder_Click;
            // 
            // lblTotal
            // 
            lblTotal.Location = new Point(32, 350);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(300, 29);
            lblTotal.TabIndex = 4;
            lblTotal.Text = "Total: 0.00";
            // 
            // txtShippingAddress
            // 
            txtShippingAddress.Location = new Point(32, 400);
            txtShippingAddress.Name = "txtShippingAddress";
            txtShippingAddress.Size = new Size(300, 27);
            txtShippingAddress.TabIndex = 5;
            // 
            // lblShippingAddress
            // 
            lblShippingAddress.AutoSize = true;
            lblShippingAddress.Location = new Point(32, 370);
            lblShippingAddress.Name = "lblShippingAddress";
            lblShippingAddress.Size = new Size(120, 20);
            lblShippingAddress.TabIndex = 0;
            lblShippingAddress.Text = "Адрес доставки:";
            // 
            // CartForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(lblShippingAddress);
            Controls.Add(txtShippingAddress);
            Controls.Add(btnClose);
            Controls.Add(btnClearCart);
            Controls.Add(btnPlaceOrder);
            Controls.Add(lblTotal);
            Controls.Add(listViewCart);
            Name = "CartForm";
            Text = "Корзина";
            ResumeLayout(false);
            PerformLayout();
        }

        private ColumnHeader columnProductName;
        private ColumnHeader columnSize;
        private ColumnHeader columnQuantity;
        private ColumnHeader columnPrice;
    }
}


