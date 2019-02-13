using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public class Effect
    {
        #region Private Data
        string m_name;
        int m_amount;
        int m_duration;
        string m_caster; 
        #endregion

        public string Name
        {
            get { return m_name; }
        }

        public Effect(string info)
        {
            string[] effectInfo = info.Split(Protocol.VSPLIT.ToCharArray());
            this.m_name = effectInfo[0];
            this.m_amount = Convert.ToInt32(effectInfo[1]);
            this.m_duration = Convert.ToInt32(effectInfo[2]);
            this.m_caster = effectInfo[3];
        }
    }
}
