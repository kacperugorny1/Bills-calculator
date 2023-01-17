using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shopping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double[,] Costs = new double[4, 4]; //0 - kacper, 1 - seba, 2 - zuza, 3 - dawid
        //
        Stack<double[,]> History = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private int How_many_ppl_pay() // liczy na ilu rachunek
        {
            int ans = 0;
            if (KacperCheckBox.IsChecked == true)
                ans++;
            if (SebaCheckBox.IsChecked == true)
                ans++;
            if (DawidCheckBox.IsChecked == true)
                ans++;
            if (ZuziaCheckBox.IsChecked == true)
                ans++;
            return ans;
        }
        private void Add_price(double money, int who_paid) // dodaje kase
        {
            History.Push((double[,])Costs.Clone());
            if (KacperCheckBox.IsChecked == true)
                Costs[0, who_paid] += money;
            if (SebaCheckBox.IsChecked == true)
                Costs[1, who_paid] += money;
            if (ZuziaCheckBox.IsChecked == true)
                Costs[2, who_paid] += money;
            if (DawidCheckBox.IsChecked == true)
                Costs[3, who_paid] += money;
        }

        private void Update_price() // aktualizuje kwoty 
        {

            (double, double, double, double) bilans = Count_differences();

            KacperText.Text = $"Kacper płaci dla\nSeba: {Math.Round(Costs[0, 1],2)}\nZuzia: {Math.Round(Costs[0, 2], 2)}\nDawid: {Math.Round(Costs[0, 3], 2)}\nBilans: {Math.Round(bilans.Item1, 2)}";
            SebaText.Text = $"Seba płaci dla\nKacper: {Math.Round(Costs[1, 0], 2)}\nZuzia: {Math.Round(Costs[1, 2], 2)}\nDawid: {Math.Round(Costs[1, 3],2)}\nBilans: {Math.Round(bilans.Item2, 2)}";
            ZuziaText.Text = $"Zuzia płaci dla\nKacper: {Math.Round(Costs[2, 0], 2)}\nSeba: {Math.Round(Costs[2, 1], 2) }\nDawid: {Math.Round(Costs[2, 3], 2)}\nBilans: {Math.Round(bilans.Item3, 2)}";
            DawidText.Text = $"Dawid płaci dla\nKacper: {Math.Round(Costs[3, 0], 2)}\nSeba: {Math.Round(Costs[3, 1], 2)}\nZuzia: {Math.Round(Costs[3, 2], 2)}\nBilans: {Math.Round(bilans.Item4, 2)}";
        }

        (double, double, double, double) Count_differences()
        {
            double kacper_diff = -Costs[0, 1] - Costs[0, 2] - Costs[0, 3] + Costs[1, 0] + Costs[2, 0] + Costs[3, 0];
            double seba_diff = -Costs[1, 0] - Costs[1, 2] - Costs[1, 3] + Costs[0, 1] + Costs[2, 1] + Costs[3, 1];
            double zuzia_diff = -Costs[2, 0] - Costs[2, 1] - Costs[2, 3] + Costs[0, 2] + Costs[1, 2] + Costs[3, 2];
            double dawid_diff = -Costs[3, 0] - Costs[3, 1] - Costs[3, 2] + Costs[0, 3] + Costs[1, 3] + Costs[2, 3];
            return (kacper_diff, seba_diff, zuzia_diff, dawid_diff);
        }

        private int Who_paid() // sprawdza kto placil
        {
            int who_paid = 4;
            if (KacperRadio.IsChecked == true)
                who_paid = 0;
            if (SebaRadio.IsChecked == true)
                who_paid = 1;
            if (ZuziaRadio.IsChecked == true)
                who_paid = 2;
            if (DawidRadio.IsChecked == true)
                who_paid = 3;
            return who_paid;
        }
        private void Add_button_click(object sender, RoutedEventArgs e) //przycisk dodaj
        {
            string text_num = Cost.Text;
            text_num = text_num.Replace('.', ',');
            int who_paid = Who_paid();
            int how_many = How_many_ppl_pay();
            
            if (!double.TryParse(text_num, out double result) || who_paid == 4 || how_many == 0)
            {
                Cost.Text = "";
                return;
            }
            Cost.Text = "";
            Add_price(result / how_many, who_paid);
            Add_History_Text(who_paid, result);
            Update_price();
            
        }

        private void Add_History_Text(int who_paid, double cost)
        {
            string str;
            str = who_paid switch
            {
                0 => "K",
                1 => "S",
                2 => "Z",
                3 => "D",
                _ => "?"
            };
            str += $" {cost} ";
            if (KacperCheckBox.IsChecked == true)
                str += "K ";
            if (SebaCheckBox.IsChecked == true)
                str += "S ";
            if (ZuziaCheckBox.IsChecked == true)
                str += "Z ";
            if (DawidCheckBox.IsChecked == true)
                str += "D ";
            HistoryText.Text += str + "\n";
        }


        

        private void IsKeyDown(object sender, KeyEventArgs e) // sprawdza wcisniete przyciski
        {
            if (e.Key == Key.Enter)
            {
                Add_button_click(sender, e);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.R)
            {
                Undo(sender, e);
                return;
            }
            if (e.Key == Key.F1)
            {
                KacperCheckBox.IsChecked = !KacperCheckBox.IsChecked == true;
            }
            else if (e.Key == Key.F2)
            {
                SebaCheckBox.IsChecked = !SebaCheckBox.IsChecked == true;
            }
            else if (e.Key == Key.F3)
            {
                ZuziaCheckBox.IsChecked = !ZuziaCheckBox.IsChecked == true;
            }
            else if (e.Key == Key.F4)
            {
                DawidCheckBox.IsChecked = !DawidCheckBox.IsChecked == true;
            }
        }

        private void Undo(object sender, RoutedEventArgs e) //cofa ostatnia operacje
        {
            if (History.Count == 0)
                return;
            Costs = History.Pop();
            Update_price();
            HistoryText.Text += "Cofnij \n";
        }

        private void ClosedWindow(object sender, EventArgs e)
        {
            string path = "C:\\Users\\Kacper1\\Desktop\\Rozliczenie.txt";
            using (var File = new StreamWriter(path, true))
            {
                File.WriteLine($"Rozlizenie za {DateTime.Now}\n");
                File.WriteLine(KacperText.Text + "\n" + SebaText.Text + "\n" + ZuziaText.Text + "\n"+ DawidText.Text);
                File.WriteLine("\n\n");
            }
                
        }
        private void Count_prices(object sender, RoutedEventArgs e) // optymalizacja cen
        {
            History.Push((double[,])Costs.Clone());
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (i == j)
                        continue;
                    if (Costs[i, j] == Costs[j, i])
                    {
                        Costs[i, j] = 0;
                        Costs[j, j] = 0;
                    }
                    else if (Costs[i, j] < Costs[j, i])
                    {
                        Costs[j, i] -= Costs[i, j];
                        Costs[i, j] = 0;
                    }
                    else if (Costs[i, j] > Costs[j, i])
                    {
                        Costs[i, j] -= Costs[j, i];
                        Costs[j, i] = 0;
                    }
                }
            }
            Update_price();
            HistoryText.Text += "Optymalizuj\n";
        }
        private void Optimalize(object sender, RoutedEventArgs e)
        {
            Count_prices(sender, e);
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (i == j)
                        continue;
                    if (Costs[i, j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (k == i || k == j)
                                continue;
                            if (Costs[j, k] == 0 || Costs[i, k] == 0)
                                continue;
                            if (Costs[j, k] <= Costs[i, k])
                            {
                                Costs[i, k] += Costs[j, k];
                                Costs[i, j] -= Costs[j, k];
                                if (Costs[i,j] < 0)
                                {
                                    Costs[j, i] = -Costs[i, j];
                                    Costs[i, j] = 0;
                                }    

                                Costs[j, k] = 0;
                            }
                            else
                            {
                                Costs[i, k] += Costs[i, j];
                                Costs[j, k] -= Costs[i, j];
                                Costs[i, j] = 0;
                            }
                        }
                    }    
                }
            }

            Update_price();
        }
    }
}
    