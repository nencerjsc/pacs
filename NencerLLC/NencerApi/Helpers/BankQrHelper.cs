namespace NencerApi.Helpers
{
    public class BankQrHelper
    {

        //Bankcode dạng số nhé, VCB là 970436
        public string GenerateQrString(string bankcode, string accnum, string amount, string noidung)
        {

            string code3 = "0006" + bankcode + "01" + LenStr(accnum) + accnum;
            string code2 = "0010A00000072701" + code3.Length;
            string code4 = "0208QRIBFTTA";
            string code5 = "5303704";

            string code7 = "08" + LenStr(noidung) + noidung;
            string code6 = "54" + LenStr(amount) + amount + "5802VN62" + LenStr(code7) + code7;

            string code8 = "00020101021138" + (code2 + code3 + code4).Length + code2 + code3 + code4 + code5 + code6 + "6304";

            // Tính CRC-CCITT cho `code8`
            string code = code8 + CrcCcitt(code8);

            return code;
        }


        public static string CrcCcitt(string data, ushort poly = 0x1021, ushort initVal = 0xFFFF)
        {
            ushort crc = initVal;
            foreach (char c in data)
            {
                crc ^= (ushort)(c << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ poly);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
                crc &= 0xFFFF;
            }
            return crc.ToString("X4"); // Trả về mã CRC dạng hexa
        }

        public static string LenStr(string xau)
        {
            return xau.Length < 10 ? "0" + xau.Length : xau.Length.ToString();
        }

    }
}
