using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using System.Text.RegularExpressions;
using System.Threading;

namespace sdk
{
    public partial class Form1 : Form
    {
        Action action;
        bool exit = false;
        Int32 row, column;
        Int32 value;
        Int32[,] super_square_index = new Int32[9, 9]
        {
            { 0, 0, 0, 1, 1, 1, 2, 2, 2 }, { 0, 0, 0, 1, 1, 1, 2, 2, 2 }, { 0, 0, 0, 1, 1, 1, 2, 2, 2 },
            { 3, 3, 3, 4, 4, 4, 5, 5, 5 }, { 3, 3, 3, 4, 4, 4, 5, 5, 5 }, { 3, 3, 3, 4, 4, 4, 5, 5, 5 },
            { 6, 6, 6, 7, 7, 7, 8, 8, 8 }, { 6, 6, 6, 7, 7, 7, 8, 8, 8 }, { 6, 6, 6, 7, 7, 7, 8, 8, 8 },
        };

        Int32[,] square_index = new Int32[9, 9]
        {
            {0,1,2,0,1,2,0,1,2 }, {3,4,5,3,4,5,3,4,5 }, {6,7,8,6,7,8,6,7,8 },
            {0,1,2,0,1,2,0,1,2 }, {3,4,5,3,4,5,3,4,5 }, {6,7,8,6,7,8,6,7,8 },
            {0,1,2,0,1,2,0,1,2 }, {3,4,5,3,4,5,3,4,5 }, {6,7,8,6,7,8,6,7,8 },
        };

        List<List<Int32>> H = new List<List<Int32>> {
            new List<Int32>{0,0,0,0,0,0,0,0,0 }, new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 } };

        List<List<Int32>> V = new List<List<Int32>> {
            new List<Int32>{0,0,0,0,0,0,0,0,0 }, new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 } };

        List<List<Int32>> S = new List<List<Int32>> {
            new List<Int32>{0,0,0,0,0,0,0,0,0 }, new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },
            new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 },new List<Int32>{0,0,0,0,0,0,0,0,0 } };

        public Form1()
        {
            InitializeComponent();
            lblMsg.Text = "";
            lblElapsedms.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void Start()
        {

            try
            {
                Stopwatch sw = new Stopwatch();
                this.BeginInvoke(action = () => lblMsg.Text = "");
                sw.Start();
                clear_solution(false);
                row = 0;
                column = 0;
                value = 0;

                while (!exit && start(ref row, ref column, ref value))
                {

                }

                sw.Stop();

                this.BeginInvoke(action = () => lblElapsedms.Text = $"{sw.ElapsedMilliseconds} ms");

                show_content();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            this.BeginInvoke(action = () => btnStart.Enabled = true);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            new Thread(Start).Start(); ;
        }

        void show_content()
        {
            Int32 r, c, v;

            for (r = 0; r <= 8; r++)
            {
                for (c = 0; c <= 8; c++)
                {
                    v = read_value(r, c);
                    if (v != 0)
                    {
                        while (((TextBox)Controls[$"tb{r}{c}"]).Text.Equals(""))
                        {
                            this.Invoke(action = () =>
                            {
                                ((TextBox)Controls[$"tb{r}{c}"]).Text = Math.Abs(v).ToString();
                                ((TextBox)Controls[$"tb{r}{c}"]).Refresh();
                                Application.DoEvents();
                            });
                        }
                    }
                    else
                    {
                        clear_solution(false);
                        this.Invoke(action = () => lblMsg.Text = "No solution found");
                        return;
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            exit = true;
            Environment.Exit(0);
        }



        private void tb_Validating(object sender, CancelEventArgs e)
        {
            lblMsg.Text = "";
            int r = Convert.ToInt32(((TextBox)sender).Name[2].ToString());
            int c = Convert.ToInt32(((TextBox)sender).Name[3].ToString());

            update_value(r, c, 0);


            if (((TextBox)sender).Text == "" || ((TextBox)sender).Text == " ")
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).BackColor = System.Drawing.Color.White;
                return;
            }


            if (Int32.TryParse(((TextBox)sender).Text, out Int32 v) && ((v >= 1) && (v <= 9)) && !contains_value(r, c, v))
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.Red;
                update_value(r, c, -v); //negative because is given, not guessed
                return;
            }
            else
            {
                lblMsg.Text = "Value not valid";
                ((TextBox)sender).BackColor = System.Drawing.Color.White;
                e.Cancel = true;
            }
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(action = () =>
            {
                lblMsg.Text = "";
            });
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            clear_solution(true);
        }

        private void btnClearSolution_Click(object sender, EventArgs e)
        {
            clear_solution(false);
        }

        void clear_solution(bool clear_all)
        {
            this.BeginInvoke(action = () =>
            {
                lblMsg.Text = "";
                lblElapsedms.Text = "";
            });
            if (clear_all)
            {
                //clear all boxes
                foreach (Control ctrl in this.Controls) // get controls
                {
                    if (ctrl.GetType() == typeof(TextBox)) // of type textbox
                    {
                        this.BeginInvoke(action = () =>
                        {
                            ((TextBox)ctrl).Text = "";
                            ((TextBox)ctrl).BackColor = System.Drawing.Color.White;
                        });

                        update_value(
                            Convert.ToInt32(ctrl.Name[2].ToString()), //row
                            Convert.ToInt32(ctrl.Name[3].ToString()), //column
                            0);


                    }
                }
            }
            else
            {
                //clear white boxes
                foreach (Control ctrl in this.Controls) // get controls
                {
                    if (ctrl.GetType() == typeof(TextBox) && ((TextBox)ctrl).BackColor == System.Drawing.Color.White) // of type textbox
                    {
                        this.BeginInvoke(action = () =>
                        {
                            ((TextBox)ctrl).Text = "";
                        });

                        update_value(
                            Convert.ToInt32(ctrl.Name[2].ToString()), //row
                            Convert.ToInt32(ctrl.Name[3].ToString()), //column
                            0);
                    }
                }
            }
        }

        bool write_value(Int32 row, Int32 column, Int32 value)
        {

            //Init values are negative
            if (H[row][column] < 0)
            {
                return false;
            }


            //Invalid value
            if (!((Math.Abs(value) >= 0) && (Math.Abs(value) <= 9)))
            {
                return false;
            }


            //Remove previous value
            update_value(row, column, 0);

            if (cbDisplayEvolution.Checked)
            {

                this.Invoke(action = () =>
                {
                          ((TextBox)Controls[$"tb{row}{column}"]).Text = "";
                          ((TextBox)Controls[$"tb{row}{column}"]).Refresh();
                });

            }

            if (contains_value(row, column, value))
            {
                return false;
            }

            update_value(row, column, value);

            if (cbDisplayEvolution.Checked)
            {

                this.Invoke(action = () =>
                {
                          ((TextBox)Controls[$"tb{row}{column}"]).Text = Math.Abs(value).ToString();
                          ((TextBox)Controls[$"tb{row}{column}"]).Refresh();
                });

            }
            return true;
        }

        void update_value(Int32 row, Int32 column, Int32 value)
        {
            H[row][column] = value;
            V[column][row] = value;
            S[get_super_square_index(row, column)][get_square_index(row, column)] = value;


        }

        bool contains_value(Int32 row, Int32 column, Int32 value)
        {
            if (H[row].Contains(value) || H[row].Contains(-value))
            {
                return true;
            }

            if (V[column].Contains(value) || V[column].Contains(-value))
            {
                return true;
            }

            if (S[get_super_square_index(row, column)].Contains(value) || S[get_super_square_index(row, column)].Contains(-value))
            {
                return true;
            }
            return false;
        }

        bool start(ref Int32 row, ref Int32 column, ref Int32 value)
        {
            if (read_value(row, column) >= 0)
            {
                if (write_value(row, column, value))
                {
                    if (increase_column(ref column, ref row))
                    {
                        value = 1;
                        return true;
                    }
                }
                else //value already in row, column or square
                {
                    if (increase_value(ref column, ref row, ref value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (increase_column(ref column, ref row))
                {
                    value = 1;
                    return true;
                }
            }
            return false;
        }


        Int32 read_value(Int32 row, Int32 column)
        {
            if (H[row][column] == V[column][row] && H[row][column] == S[get_super_square_index(row, column)][get_square_index(row, column)])
            {
                return H[row][column];
            }
            else
            {
                throw new Exception();
            }

        }

        Int32 get_super_square_index(Int32 row, Int32 column)
        {
            return super_square_index[row, column];

        }

        Int32 get_square_index(Int32 row, Int32 column)
        {
            return square_index[row, column];

        }



        bool increase_column(ref Int32 column, ref Int32 row)
        {
            column++;
            if (column <= 8)
            {
                return true;
            }
            else
            {
                column = 0;
                if (increase_row(ref row))
                {
                    return true;
                }
            }
            return false;
        }

        bool decrease_column(ref Int32 column, ref Int32 row)
        {
            column--;
            if (column >= 0)
            {
                if (read_value(row, column) >= 0)
                {
                    return true;
                }
                else //value can't be changed 
                {
                    if (decrease_column(ref column, ref row))
                    {
                        return true;
                    }
                }
            }
            else
            {
                column = 8;
                if (decrease_row(ref row))
                {
                    if (read_value(row, column) >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        if (decrease_column(ref column, ref row))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool decrease_row(ref Int32 row)
        {
            row--;
            if (row >= 0)
            {
                return true;
            }

            return false;
        }

        bool increase_row(ref Int32 row)
        {
            row++;
            if (row <= 8)
            {
                return true;
            }

            return false;
        }



        bool increase_value(ref Int32 column, ref Int32 row, ref Int32 value)
        {
            value++;

            if (value <= 9)
            {
                return true;
            }
            else // value too big
            {
                write_value(row, column, 0);

                if (decrease_column(ref column, ref row)) //move to previous column with value >=0
                {
                    value = read_value(row, column);

                    if (increase_value(ref column, ref row, ref value))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else//column invalid
                {
                    if (decrease_row(ref row))
                    {
                        column = 8;
                        return true;
                    }
                    else //row is invalid
                    {
                        return false;
                    }
                }
            }

        }
    }
}
