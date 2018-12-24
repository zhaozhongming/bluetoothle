using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Infrastructure
{
    class FlowCom3000Command
    {
        public static readonly byte PASSWORD = 0x29;
        public static readonly byte GET_SETUPENTRY = 0x2A;
        public static readonly byte GET_TABLEENTRY = 0x2B;
        public static readonly byte GET_SAVE_SETUP = 0x2C;
        public static readonly byte PRINTFORM = 0x2D;
        public static readonly byte STARTSTOP = 0x2E;
        public static readonly byte GET_KEY = 0x2F;
        public static readonly byte GET_PRINTLINE = 0x30;
        public static readonly byte GET_PASSWORD = 0x31;
        public static readonly byte GET_SYNC_TIME = 0x32;
        public static readonly byte GET_PORT = 0x33;
        public static readonly byte GET_WRITE_EEPROM = 0x34;
        public static readonly byte GET_READ_EEPROM = 0x35;
        public static readonly byte GET_CONTRAST = 0x36;
        public static readonly byte GET_LOGGER_NO = 0x37;
        public static readonly byte PUT_SETUPENTRY = 0x38;
        public static readonly byte PUT_STRINGLIST = 0x39;

        public static readonly byte RESET = (byte)0x80;
        public static readonly byte PUT_CLRBRNET = (byte)0x81;
        public static readonly byte PUT_CLRTOTAL = (byte)0x82;
        public static readonly byte PUT_SETUPHEADER = (byte)0x83;
        public static readonly byte KORRTAB = (byte)0x84;
        public static readonly byte PUT_DATA = (byte)0x85;
        public static readonly byte PUT_STATUS = (byte)0x86;
        public static readonly byte PUT_TRANSACTION_HEADER = (byte)0x87;
        public static readonly byte PUT_TRANSACTION_RESULT = (byte)0x88;
        public static readonly byte PUT_METER_DATA = (byte)0x89;
        public static readonly byte BRUTTO = (byte)0x8A;
        public static readonly byte NETTO = (byte)0x8B;
        public static readonly byte SENSOR = (byte)0x8C;
        public static readonly byte PUT_RELEASE = (byte)0x8D;
        public static readonly byte PUT_DYNAMISCH = (byte)0x8E;
        public static readonly byte PUT_CLRNETTO = (byte)0x8F;
        public static readonly byte SERIALNO = (byte)0x90;
        public static readonly byte EVENTCTR = (byte)0x91;
        public static readonly byte PUT_DATA_TG1 = (byte)0x92;
        public static readonly byte PUT_PORT = (byte)0x93;
        public static readonly byte PUT_ANALOG = (byte)0x94;
        public static readonly byte PUT_TIME = (byte)0x95;
        public static readonly byte PUT_DATA_TG2 = (byte)0x96;
        public static readonly byte PUT_DATA_TG3 = (byte)0x97;
        public static readonly byte PUT_TOTALIZER = (byte)0x98;
        public static readonly byte PUT_CAR_NO = (byte)0x99;
        public static readonly byte PUT_LOGGER_CNT = (byte)0x9A;
        public static readonly byte PUT_LOGGER_CLR = (byte)0x9B;
    }
}
