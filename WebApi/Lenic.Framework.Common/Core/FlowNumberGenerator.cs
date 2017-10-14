using System;
using System.Collections.Generic;
using System.Linq;

namespace Lenic.Framework.Common.Core
{
    /// <summary>
    /// 流水号生成器
    /// </summary>
    internal class FlowNumberGenerator
    {
        #region Private Fields

        private static readonly int ZeroCharValue = (int)'0';

        #endregion Private Fields

        #region Business Properties

        public List<char> DataSource { get; private set; }

        #endregion Business Properties

        #region Entrance

        public FlowNumberGenerator(string obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            DataSource = obj.ToList();
        }

        #endregion Entrance

        #region Business Methods

        public void Build(int increment = 1)
        {
            int index = -1;
            for (int i = (DataSource.Count - 1); i >= 0; i--)
            {
                if (!char.IsWhiteSpace(DataSource[i]))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                IncreaseFlowNumber(0, increment, true);
            else
            {
                if (char.IsDigit(DataSource[index]))
                    IncreaseFlowNumber(index, increment, false);
                else
                    IncreaseFlowNumber(index + 1, increment, true);
            }
        }

        public override string ToString()
        {
            return new string(DataSource.ToArray());
        }

        #endregion Business Methods

        #region Private Methods

        private void IncreaseFlowNumber(int index, int increment, bool? isInsert = null)
        {
            if (increment <= 0)
                return;

            int current = increment % 10;
            int other = (increment - current) / 10;

            if (index < 0 || (isInsert.HasValue && isInsert.Value))
            {
                DataSource.Insert((index < 0 ? 0 : index), ConvertToChar(current));
                IncreaseFlowNumber(index - 1, other);
            }
            else
            {
                var currentChar = DataSource[index];
                if (!char.IsDigit(currentChar))
                {
                    DataSource.Insert(index + 1, ConvertToChar(current));
                    IncreaseFlowNumber(index, other, true);
                }
                else
                {
                    var value = current + ConvertToInt(currentChar);
                    other += value / 10;
                    value = value % 10;
                    DataSource[index] = ConvertToChar(value);
                    IncreaseFlowNumber(index - 1, other);
                }
            }
        }

        private char ConvertToChar(int value)
        {
            return (char)(value + ZeroCharValue);
        }

        private int ConvertToInt(char value)
        {
            return ((int)value) - ZeroCharValue;
        }

        #endregion Private Methods
    }
}