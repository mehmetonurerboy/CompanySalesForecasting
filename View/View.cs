using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using salesPrediction.algorithms;

namespace salesPrediction.View
{
    public partial class View : Form
    {

        private String code;
        private string targetPeriod;
        private string year;
        private string quarter;
        private double[] movAve;
        private double[] expSmoth;
        private double multReg;
        private double priceAnnAve;
        private double[] actualData;
        private int dataPercentageValue;

        public View()
        {
            InitializeComponent();
        }

        private void View_Load(object sender, EventArgs e)
        {
            var codeList = algorithmService.GetCodeList();
            for (int i = 0; i < codeList.Count; i++)
            {
                companySelection.Items.Add(codeList.ElementAt(i).GetElement("_id").Value);
            }

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            code = Convert.ToString(companySelection.SelectedItem);
        }

        private void companyChoiceGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void companyChoicePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
       
        
        

        private void companyChoiceNextButton_Click(object sender, EventArgs e)
        {
            if(companySelection.SelectedItem != null)
            {
                targetTimeSelectionPanel.Visible = true;
            }
            else
            {
                MessageBox.Show("LÜTFEN ŞİRKET SEÇİNİZ!");
            }
        }
        
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (companySelection.SelectedItem != null)
            {
                targetTimeSelectionPanel.Visible = true;
            }
            else
            {
                MessageBox.Show("ŞİRKET SEÇMEDİNİZ! LÜTFEN BİR ŞİRKET SEÇİN!");
            }
        }

        private void yearInput_TextChanged_1(object sender, EventArgs e)
        {
            int value;
            if (Int32.TryParse(yearInput.Text, out value))
            {
                value = Convert.ToInt32(yearInput.Text);
                if ((value > 1987) && (value < 2019))
                {
                    year = Convert.ToString(value);
                }
                else
                {
                    year = null;
                }
            }
            else
            {
                MessageBox.Show("YIL DEĞERİ KISMINA HARF GİREMEZSİNİZ!");
                yearInput.Clear();
            }
            
        }

        private void targetTimeSelectionPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void targetDateBackButton_Click_1(object sender, EventArgs e)
        {
            targetTimeSelectionPanel.Visible = false;
        }

        private void quarterInput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int period = 3 + 3 * quarterInput.SelectedIndex;
            if (period < 12)
            {
                quarter = '0' + Convert.ToString(period);
            }
            else
            {
                quarter = Convert.ToString(period);
            }
        }

        private void targetDateNextButton_Click_1(object sender, EventArgs e)
        {
            if ((year != null) && (quarterInput.SelectedItem != null))
            {
                targetPeriod = year + '-' + quarter;
                algorithmChoicePanel.Visible = true;
                actualData = algorithmService.GetDifferentiatedData(algorithmService.GetDataAll(code, targetPeriod), "#386-BRUT SATISLAR");
            }
            else
            {
                MessageBox.Show("VERİLER DOĞRU GİRİLMEMİŞ!");
            }
        }

        private void algorithmChoiceBackButton_Click(object sender, EventArgs e)
        {
            algorithmChoicePanel.Visible = false;
        }

        private void movingAverageInput_CheckedChanged(object sender, EventArgs e)
        {
            if(movingAverageInput.Checked == true)
            {
                movingAveragePeriyotInput.Enabled = true;
            }
            else
            {
                movingAveragePeriyotInput.Enabled = false;
                movAve = null;
            }
        }

        private void exponentialSmoothingInput_CheckedChanged(object sender, EventArgs e)
        {
            if(exponentialSmoothingInput.Checked == true)
            {
                exponentialSmoothingFactorInput.Enabled = true;
            }
            else
            {
                exponentialSmoothingFactorInput.Enabled = false;
                expSmoth = null;
            }
        }

        private void regressionInput_CheckedChanged(object sender, EventArgs e)
        {
            if(regressionInput.Checked == true)
            {

            }
            else
            {

                multReg = -1;
            }
        }

        private void priceAnnualAverageInput_CheckedChanged(object sender, EventArgs e)
        {
            if(priceAnnualAverageInput.Checked == true)
            {

            }
            else
            {

                priceAnnAve = -1;
            }
        }

        private void algorithmChoiceNextButton_Click(object sender, EventArgs e)
        {
            if((movingAverageInput.Checked == true) || (exponentialSmoothingInput.Checked == true) || (regressionInput.Checked == true) || (priceAnnualAverageInput.Checked == true))
            {
                parameterChoicePanel.Visible = true;
            }
            else
            {
                MessageBox.Show("LÜTFEN EN AZ BİR ALGORİTMAYI SEÇİNİZ!");
            }
        }

        private void parameterChoiceBackButton_Click(object sender, EventArgs e)
        {
            parameterChoicePanel.Visible = false;
        }

        private void parameterChoiceNextButton_Click(object sender, EventArgs e)
        {
            if(movingAverageInput.Checked == true)
            {
                movAve = MovingAverage.MA(code, targetPeriod, Convert.ToInt32(movingAveragePeriyotInput.Value), -1, Convert.ToInt32(dataPercentage.Value));
                movingAverageChart.Visible = true;
                movingAverageChart.Series[1].Points.AddY(actualData[actualData.Length - 1]);
                movingAverageChart.Series[0].Points.AddY(movAve[movAve.Length - 1]);

                movAveRealData.Visible = true;
                movAvePredictValue.Visible = true;
                movAveDifferValue.Visible = true;
                movAvePercDifferValue.Visible = true;
                movAveRealData.Text = Convert.ToString(actualData[actualData.Length - 1]);
                movAvePredictValue.Text = Convert.ToString(movAve[movAve.Length - 1]);
                movAveDifferValue.Text = Convert.ToString((actualData[actualData.Length - 1] - movAve[movAve.Length - 1]));
                movAvePercDifferValue.Text = Convert.ToString(((actualData[actualData.Length - 1] - movAve[movAve.Length - 1]) / actualData[actualData.Length - 1]));
            }
            else
            {
                movAve = null;
                movingAverageChart.Visible = false;
                movAveRealData.Visible = false;
                movAvePredictValue.Visible = false;
                movAveDifferValue.Visible = false;
                movAvePercDifferValue.Visible = false;
            }

            if(exponentialSmoothingInput.Checked == true)
            {
                expSmoth = ExponentialSmoothing.ES(code, targetPeriod, Convert.ToDouble(exponentialSmoothingFactorInput.Value / 100), -1, Convert.ToInt32(dataPercentage.Value));
                exponentialSmoothingChart.Visible = true;
                exponentialSmoothingChart.Series[1].Points.AddY(actualData[actualData.Length - 1]);
                exponentialSmoothingChart.Series[0].Points.AddY(expSmoth[expSmoth.Length - 1]);

                expSmootRealData.Visible = true;
                expSmootPredictValue.Visible = true;
                expSmootDifferValue.Visible = true;
                expSmootPercDifferValue.Visible = true;
                expSmootRealData.Text = Convert.ToString(actualData[actualData.Length - 1]);
                expSmootPredictValue.Text = Convert.ToString(expSmoth[expSmoth.Length - 1]);
                expSmootDifferValue.Text = Convert.ToString((actualData[actualData.Length - 1] - expSmoth[expSmoth.Length - 1]));
                expSmootPercDifferValue.Text = Convert.ToString(((actualData[actualData.Length - 1] - expSmoth[expSmoth.Length - 1]) / actualData[actualData.Length - 1]));
            }
            else
            {
                expSmoth = null;
                exponentialSmoothingChart.Visible = false;
                expSmootRealData.Visible = false;
                expSmootPredictValue.Visible = false;
                expSmootDifferValue.Visible = false;
                expSmootPercDifferValue.Visible = false;

            }

            if(regressionInput.Checked == true)
            {
                multReg = Regression.MultipleLinearRegression(code, "165-TOPLAM AKTIFLER", "395-SATIS GELIRLERI", targetPeriod, -1, Convert.ToInt32(dataPercentage.Value));
                multipleRegressionChart.Visible = true;
                multipleRegressionChart.Series[1].Points.AddY(actualData[actualData.Length - 1]);
                multipleRegressionChart.Series[0].Points.AddY(multReg);

                multRegRealData.Visible = true;
                multRegPredictValue.Visible = true;
                MultRegDifferValue.Visible = true;
                multRegPercDifferValue.Visible = true;
                multRegRealData.Text = Convert.ToString(actualData[actualData.Length - 1]);
                multRegPredictValue.Text = Convert.ToString(multReg);
                MultRegDifferValue.Text = Convert.ToString((actualData[actualData.Length - 1] - multReg));
                multRegPercDifferValue.Text = Convert.ToString(((actualData[actualData.Length - 1] - multReg) / multReg));
            }
            else
            {
                multReg = -1;
                multipleRegressionChart.Visible = false;
                multRegRealData.Visible = false;
                multRegPredictValue.Visible = false;
                MultRegDifferValue.Visible = false;
                multRegPercDifferValue.Visible = false;
            }

            if(priceAnnualAverageInput.Checked == true)
            {
                priceAnnAve = PriceAnnualAverage.PAA(code, targetPeriod, -1, Convert.ToInt32(dataPercentage.Value));
                priceQuantityAnnualAverageChart.Visible = true;
                priceQuantityAnnualAverageChart.Series[1].Points.AddY(actualData[actualData.Length - 1]);
                priceQuantityAnnualAverageChart.Series[0].Points.AddY(priceAnnAve);

                priceAnnAveRealData.Visible = true;
                priceAnnAvePredictValue.Visible = true;
                priceAnnAveDifferValue.Visible = true;
                priceAnnAvePercDifferValue.Visible = true;
                priceAnnAveRealData.Text = Convert.ToString(actualData[actualData.Length - 1]);
                priceAnnAvePredictValue.Text = Convert.ToString(priceAnnAve);
                priceAnnAveDifferValue.Text = Convert.ToString((actualData[actualData.Length - 1]) - priceAnnAve);
                priceAnnAvePercDifferValue.Text = Convert.ToString(((actualData[actualData.Length - 1] - priceAnnAve) / priceAnnAve));
            }
            else
            {
                priceAnnAve = -1;
                priceQuantityAnnualAverageChart.Visible = false;

                priceAnnAveRealData.Visible = false;
                priceAnnAvePredictValue.Visible = false;
                priceAnnAveDifferValue.Visible = false;
                priceAnnAvePercDifferValue.Visible = false;
            }

            

            resultPanel.Visible = true;
            
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            //if(movingAveragePeriyotInput.)
        }

        private void movingAverageNumUpDown_ValueChanged(object sender, EventArgs e)
        {

        }
        

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void resultPanelGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void resultOutput_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void resultPanelBackButton_Click(object sender, EventArgs e)
        {
            resultPanel.Visible = false;

            multipleRegressionChart.Series[0].Points.RemoveAt(1);
            multipleRegressionChart.Series[1].Points.RemoveAt(1);

            priceQuantityAnnualAverageChart.Series[0].Points.RemoveAt(1);
            priceQuantityAnnualAverageChart.Series[1].Points.RemoveAt(1);

            movingAverageChart.Series[0].Points.RemoveAt(1);
            movingAverageChart.Series[1].Points.RemoveAt(1);

            exponentialSmoothingChart.Series[0].Points.RemoveAt(1);
            exponentialSmoothingChart.Series[1].Points.RemoveAt(1);
        }

        private void resultPanelHomepageButton_Click(object sender, EventArgs e)
        {
            resultPanel.Visible = false;
            algorithmChoicePanel.Visible = false;
            parameterChoicePanel.Visible = false;
            targetTimeSelectionPanel.Visible = false;

            regressionInput.Checked = false;
            multipleRegressionChart.Series[0].Points.RemoveAt(1);
            multipleRegressionChart.Series[1].Points.RemoveAt(1);
            multipleRegressionChart.Visible = false;

            multRegRealData.Visible = false;
            multRegPredictValue.Visible = false;
            MultRegDifferValue.Visible = false;
            multRegPercDifferValue.Visible = false;


            priceAnnualAverageInput.Checked = false;
            priceQuantityAnnualAverageChart.Series[0].Points.RemoveAt(1);
            priceQuantityAnnualAverageChart.Series[1].Points.RemoveAt(1);
            priceQuantityAnnualAverageChart.Visible = false;

            priceAnnAveRealData.Visible = false;
            priceAnnAvePredictValue.Visible = false;
            priceAnnAveDifferValue.Visible = false;
            priceAnnAvePercDifferValue.Visible = false;


            movingAverageInput.Checked = false;
            movingAverageChart.Series[0].Points.RemoveAt(1);
            movingAverageChart.Series[1].Points.RemoveAt(1);
            movingAverageChart.Visible = false;
            
            movAveRealData.Visible = false;
            movAvePredictValue.Visible = false;
            movAveDifferValue.Visible = false;
            movAvePercDifferValue.Visible = false;
            movingAveragePeriyotInput.Value = 3;


            exponentialSmoothingInput.Checked = false;
            exponentialSmoothingChart.Series[0].Points.RemoveAt(1);
            exponentialSmoothingChart.Series[1].Points.RemoveAt(1);
            exponentialSmoothingChart.Visible = false;

            expSmootRealData.Visible = false;
            expSmootPredictValue.Visible = false;
            expSmootDifferValue.Visible = false;
            expSmootPercDifferValue.Visible = false;
            exponentialSmoothingFactorInput.Value = 90;


            dataPercentage.Value = 100;
            quarterInput.SelectedItem = null;
            yearInput.Text = "";
            companySelection.SelectedItem = null;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void dataPercentage_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
