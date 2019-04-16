using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Text;

namespace Yuusha
{
    public class CharGen
    {
        public const int ACCOUNT_MIN_LENGTH = 5;
        public const int ACCOUNT_MAX_LENGTH = 12;
        public const int PASSWORD_MIN_LENGTH = 6;
        public const int PASSWORD_MAX_LENGTH = 20;

        public static string NewAccountName = "";
        public static string NewAccountPassword = "";
        public static string NewAccountEmail = "";

        public static bool FirstCharacter = false;

        private static int m_sendAttempts = 0;
        private static int m_totalRolls = 0;
        private static int m_totalAutoRolls = 0;
        private Character newbie; // pulled from World.newbies to list starting information
        private string genderToSend = "Male";
        private bool autoRollerActive = false;
        private System.Timers.Timer autoRollerTimer = new System.Timers.Timer();
        private DateTime autoRollerStartTime = new DateTime();

        private void autoRollerTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now.Subtract(autoRollerStartTime);
            //this.lblTimeElapsed.Text = elapsedTime.Days + "d " + elapsedTime.Minutes + "m " + elapsedTime.Seconds + "s";
        }

        public void enterCharGen()
        {
            m_sendAttempts = 0;
            m_totalRolls = 0;
            m_totalAutoRolls = 0;
            PopulateClassesListBox();
            PopulateHomelandsListBox();
        }

        private void PopulateClassesListBox()
        {
            //this.lstbxClasses.Items.Clear();

            foreach (Character.ClassType classType in Enum.GetValues(typeof(Character.ClassType)))
            {
                if (classType != Character.ClassType.None && classType < Character.ClassType.Knight)
                {
                    //this.lstbxClasses.Items.Add(Utility.FormatEnumString(classType.ToString()));
                }
            }
        }

        private void PopulateHomelandsListBox()
        {
            //this.lstbxHomelands.Items.Clear();

            foreach (Character.HomelandType homeland in Enum.GetValues(typeof(Character.HomelandType)))
            {
                //this.lstbxHomelands.Items.Add(homeland.ToString());
            }
        }

        private void chkAutoRoller_CheckedChanged(object sender, EventArgs e)
        {
            //this.grpbxAutoRoller.Enabled = chkAutoRoller.Checked;
            if (this.autoRollerActive)
            {
                stopAutoRoller();
            }
        }

        private void lstbxClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (lstbxHomelands.SelectedItem != null && lstbxClasses.SelectedItem != null)
            //{
            //    this.setNewbie(World.GetNewbie(lstbxHomelands.SelectedItem.ToString(), lstbxClasses.SelectedItem.ToString()));
            //}
        }

        private void lstbxHomelands_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (lstbxHomelands.SelectedItem != null && lstbxClasses.SelectedItem != null)
            //{
            //    this.setNewbie(World.GetNewbie(lstbxHomelands.SelectedItem.ToString(), lstbxClasses.SelectedItem.ToString()));
            //}
        }

        private void setNewbie(Character newbie)
        {
            if (newbie != null)
            {
                this.newbie = newbie;
                this.SetStartingAlignment();
                this.SetStartingSkills();
                this.SetStartingEquipment();
                this.SetStartingSpells();
            }
        }

        private void rbtnGenderMale_CheckedChanged(object sender, EventArgs e)
        {
            //if (rbtnGenderMale.Checked)
            //{
            //    this.genderToSend = "Male";
            //}
            //else
            //{
            //    this.genderToSend = "Female";
            //}
        }

        private void SetStartingAlignment()
        {
            //this.lblAlignment.Text = this.newbie.Alignment.ToString();
        }

        private void SetStartingSkills()
        {
            //this.lblSkillNames.Text = "";
            //this.lblStartingSkills.Text = "";

            foreach (Character.SkillType skillType in Enum.GetValues(typeof(Character.SkillType)))
            {
                if (skillType > Character.SkillType.None)
                {
                    //lblSkillNames.Text += Utility.FormatEnumString(skillType.ToString()) + ":\n";
                    //lblStartingSkills.Text += Rules.getSkillLevelTitle(skillType, newbie.Profession, newbie.GetSkillExperience(skillType), newbie.Gender) + " (" + Rules.getSkillLevel(newbie.GetSkillExperience(skillType)) + ")\n";
                }
            }
        }

        private void SetStartingEquipment()
        {
            //this.lblStartingEquipment.Text = "";
            foreach (Item item in newbie.Inventory)
            {
                //this.lblStartingEquipment.Text += item.notes + "\n";
            }
        }

        private void SetStartingSpells()
        {
            if (!newbie.IsSpellUser)
            {
                //this.grpbxStartingSpells.Visible = false;
                //this.lblManaText.Visible = false;
                //this.lblManaTextAutoRoller.Visible = false;
                //this.numMana.Visible = false;
                //this.lblMana.Visible = false;
            }
            else
            {
                //this.lblStartingSpells.Text = "";
                //foreach (Spell spell in newbie.Spells)
                //{
                //    lblStartingSpells.Text += spell.getName() + "\n";
                //}
                //this.grpbxStartingSpells.Visible = true;
                //this.lblManaText.Visible = true;
                //this.lblManaTextAutoRoller.Visible = true;
                //this.numMana.Visible = true;
                //this.lblMana.Visible = true;
            }
        }

        public void sendCharGenInfo()
        {
            m_sendAttempts++;
            if (m_sendAttempts > 25)
            {
                // produce an error
                return;
            }

            //IO.Send(Protocol.CHARGEN_RECEIVE + " " + this.genderToSend + Protocol.VSPLIT + lstbxHomelands.SelectedItem.ToString() + Protocol.VSPLIT +
            //    lstbxClasses.SelectedItem.ToString() + Protocol.VSPLIT + tbxName.Text);
        }

        public void ParseRollerResults(string results)
        {
            //this.m_totalRolls++;
            //this.lblTotalRolls.Text = this.m_totalRolls.ToString();

            //string[] resultsInfo = results.Split(Protocol.VSPLIT.ToCharArray());

            //this.lblStrength.Text = resultsInfo[0];
            //this.lblDexterity.Text = resultsInfo[1];
            //this.lblIntelligence.Text = resultsInfo[2];
            //this.lblWisdom.Text = resultsInfo[3];
            //this.lblConstitution.Text = resultsInfo[4];
            //this.lblCharisma.Text = resultsInfo[5];
            //this.lblStrengthAdd.Text = resultsInfo[6];
            //this.lblDexterityAdd.Text = resultsInfo[7];
            //this.lblHits.Text = resultsInfo[8];
            //this.lblStamina.Text = resultsInfo[9];
            //this.lblMana.Text = resultsInfo[10];

            // now check auto roller if active
            if (this.autoRollerActive)
            {
                if (!this.IsAutoRollerSatisfied())
                {
                    IO.Send("y");
                    //this.m_totalAutoRolls++;
                    //this.lblTotalAutoRolls.Text = this.m_totalAutoRolls.ToString();
                }
                else
                {
                    this.AutoRollerSuccess();
                }
            }

        }

        private void AutoRollerSuccess()
        {
            //if (chkToggleSound.Checked)
            //{
            //    try
            //    {
            //        Sound.Play("0219");
            //        //SoundEffect effect = new SoundEffect(Essence.client.DirectSoundDevice, "KSND0219.WAV");
            //        //effect.Play();
            //    }
            //    catch (Exception e)
            //    {
            //        Utility.LogException(e);
            //    }
            //}

            //if (chkToggleLogoff.Checked)
            //{
            //    Essence.client.Close();
            //}
            //else
            //{
            //    this.stopAutoRoller();
            //}
        }

        public void AcceptStepOne()
        {
            //this.splitContainer1.Panel2.Enabled = true;
        }

        public void DenyStepOne()
        {
            //this.splitContainer1.Panel1.Enabled = true;
            //this.tbxName.SelectAll();
        }

        private void btnAcceptStepOne_Click(object sender, EventArgs e)
        {
            //this.splitContainer1.Panel1.Enabled = false;
            sendCharGenInfo();
        }

        private void btnRollAgain_Click(object sender, EventArgs e)
        {
            //if (this.IsAutoRollerSatisfied() && this.m_totalAutoRolls > 0)
            //{
            //    System.Windows.Forms.DialogResult result = MessageBox.Show("Your rolled stats currently meet the auto roller requirements. Are you sure you want to start the auto roller?",
            //        "Confirm Auto Roller Start", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //    if (result != DialogResult.Yes)
            //    {
            //        return;
            //    }
            //}

            IO.Send("y");
        }

        private void btnAcceptStepTwo_Click(object sender, EventArgs e)
        {
            IO.Send("n");
            //Essence.client.login.SetLoginStatus(Login.GameState.Menu, true);
        }

        private void startAutoRoller()
        {
            //this.btnToggleAutoRoller.Text = "Stop";
            //this.btnRollAgain.Enabled = false;
            //this.btnAcceptStepTwo.Enabled = false;
            //this.btnGoBack.Enabled = false;
            this.autoRollerStartTime = DateTime.Now;
            this.autoRollerTimer.Start();
            IO.Send("y");
        }

        private void stopAutoRoller()
        {
            this.autoRollerActive = false;
            //this.btnToggleAutoRoller.Text = "Start";
            //this.btnRollAgain.Enabled = true;
            //this.btnAcceptStepTwo.Enabled = true;
            //this.btnGoBack.Enabled = true;
            this.autoRollerTimer.Stop();
        }

        private void btnToggleAutoRoller_Click(object sender, EventArgs e)
        {
            if (this.IsAutoRollerSatisfied())
            {
                //System.Windows.Forms.DialogResult result = MessageBox.Show("Your rolled stats currently meet the auto roller requirements. Are you sure you want to start the auto roller?",
                //    "Confirm Auto Roller Start", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                //if (result != DialogResult.Yes)
                //{
                //    return;
                //}
            }

            this.autoRollerActive = !this.autoRollerActive;

            if (this.autoRollerActive)
            {
                this.startAutoRoller();
            }
            else
            {
                this.stopAutoRoller();
            }
        }

        private bool IsAutoRollerSatisfied()
        {
            //// strength
            //if (Convert.ToInt32(lblStrength.Text) < this.numStrength.Value)
            //{
            //    return false;
            //}
            //// dexterity
            //if (Convert.ToInt32(lblDexterity.Text) < this.numDexterity.Value)
            //{
            //    return false;
            //}
            //// intelligence
            //if (Convert.ToInt32(lblIntelligence.Text) < this.numIntelligence.Value)
            //{
            //    return false;
            //}
            //// wisdom
            //if (Convert.ToInt32(lblWisdom.Text) < this.numWisdom.Value)
            //{
            //    return false;
            //}
            //// constitution
            //if (Convert.ToInt32(lblConstitution.Text) < this.numConstitution.Value)
            //{
            //    return false;
            //}
            //// charisma
            //if (Convert.ToInt32(lblCharisma.Text) < this.numCharisma.Value)
            //{
            //    return false;
            //}
            //// hits
            //if (Convert.ToInt32(lblHits.Text) < this.numHits.Value)
            //{
            //    return false;
            //}
            //// stamina
            //if (Convert.ToInt32(lblStamina.Text) < this.numStamina.Value)
            //{
            //    return false;
            //}
            //// mana
            //if (newbie.IsSpellUser)
            //{
            //    if (Convert.ToInt32(lblMana.Text) < this.numMana.Value)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            IO.Send(Protocol.GOTO_CHARGEN);
        }

        private void CharGen_FormClosing()
        {
            //if (e.CloseReason != CloseReason.ApplicationExitCall)
            //{
            //    if (Account.Characters.Count <= 0)
            //    {
            //        System.Windows.Forms.DialogResult result = MessageBox.Show("You currently do not have any characters on your account. If you close character generation now you will be " +
            //            "logged out automatically. You may create your first character the next time you log on. Are you sure you want to close character generation?", "Confirm Close",
            //            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            //        if (result == DialogResult.Yes)
            //        {
            //            Essence.client.login.SetLoginStatus(Login.GameState.Disconnected, true);
            //        }
            //        else
            //        {
            //            e.Cancel = true;
            //            return;
            //        }
            //    }
            //    if (this.m_totalAutoRolls > 0 && this.IsAutoRollerSatisfied())
            //    {
            //        System.Windows.Forms.DialogResult result = MessageBox.Show("Your rolled stats currently meet the auto roller requirements. Are you sure you want to close character generation?",
            //            "Confirm Auto Roller Start", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //        if (result != DialogResult.Yes)
            //        {
            //            e.Cancel = true;
            //            return;
            //        }
            //    }

            //    if (this.previousLoginStatus == Login.GameState.Conference)
            //    {
            //        Essence.client.login.SetLoginStatus(Login.GameState.Conference, true);
            //        IO.Send(Protocol.GOTO_CONFERENCE);
            //    }
            //    else
            //    {
            //        Essence.client.login.SetLoginStatus(Login.GameState.Menu, true);
            //        IO.Send(Protocol.GOTO_MENU);
            //    }

            //    exitCharGen();
        }
    }
}