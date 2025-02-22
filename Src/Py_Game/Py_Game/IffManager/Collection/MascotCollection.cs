﻿using PangyaAPI.BinaryModels;
using Py_Game.IffManager.Data.Mascot;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.IffManager.Collection
{
    public class MascotCollection : Dictionary<uint, IffMascotData>
    {
        /// <summary>
        /// ID determining relation to other IFF files
        /// </summary>
        public ushort BindingID;

        /// <summary>
        /// Version of this IFF file
        /// </summary>
        public uint Version;

        public MascotCollection()
        {
            MemoryStream Buffer;
            IffMascotData Mascot;

            PangyaBinaryReader Reader = null;

            using (var zip = ZipFile.OpenRead("data/pangya_gb.iff"))//ler o arquivo de base
            {
                var FileZip = zip.Entries.FirstOrDefault(c => c.Name == "Mascot.iff");//verifica se existe o arquivo

                if (FileZip == null)
                {
                    PangyaAPI.WriteConsole.WriteLine(" data\\Mascot.iff is not loaded");
                    return;
                }
                else
                {
                    Buffer = new MemoryStream();
                    FileZip.Open().CopyTo(Buffer);
                }
            }

            try
            {
                Reader = new PangyaBinaryReader(Buffer);
                if (new string(Reader.ReadChars(2)) == "PK")
                {
                    throw new Exception("The given IFF file is a ZIP file, please unpack it before attempting to parse it");
                }

                Reader.Seek(0, 0);

                Reader.ReadUInt16(out ushort recordCount);
                long recordLength = ((Reader.GetSize() - 8L) / (recordCount));
                Reader.ReadUInt16(out BindingID);
                Reader.ReadUInt32(out Version);

                for (int i = 0; i < recordCount; i++)
                {
                    Mascot = (IffMascotData)Reader.Read(new IffMascotData());
                    Add(Mascot.Base.TypeID, Mascot);
                }
            }
            finally
            {
                Reader.Dispose();
                Buffer.Dispose();
            }

        }

        public string GetItemName(UInt32 TypeID)
        {
            IffMascotData Mascot = new IffMascotData();
            if (!LoadMascot(TypeID, ref Mascot))
            {
                return "";
            }
            return Mascot.Base.Name;
        }

        public UInt32 GetPrice(UInt32 TypeID, uint Day)
        {
            IffMascotData Mascot = new IffMascotData();
            if (!LoadMascot(TypeID, ref Mascot))
            {
                return 0;
            }
            if (Mascot.Base.Enabled == 1)
            {
                switch (Day)
                {
                    case 1:
                        return Mascot.Price1;
                    case 7:
                        return Mascot.Price7;
                    case 30:
                        return Mascot.Price30;
                }
            }

            if (Mascot.Price1 == 0 && Mascot.Price7 == 0 && Mascot.Price30 == 0)
            {
                return Mascot.Base.PriceType;
            }
            return 0;
        }


        public sbyte GetShopPriceType(UInt32 TypeId)
        {
            IffMascotData Mascot = new IffMascotData();
            if (!LoadMascot(TypeId, ref Mascot))
            {
                return -1;
            }
            return (sbyte)Mascot.Base.PriceType;
        }
        public uint GetSalary(uint TypeId, uint Day)
        {
            IffMascotData Item = new IffMascotData();
            if (!LoadMascot(TypeId, ref Item))
            {
                return 0;
            }
            if (Item.Base.Enabled == 1)
            {
                switch (Day)
                {
                    case 1:
                        return Item.Price1;
                    case 7:
                        return Item.Price7;
                    case 30:
                        return Item.Price30;
                }
            }
            return 0;
        }
        public bool IsBuyable(UInt32 TypeId)
        {
            IffMascotData Item = new IffMascotData();
            if (!LoadMascot(TypeId, ref Item))
            {
                return false;
            }
            if (Item.Base.Enabled == 1 && Item.Base.MoneyFlag == 0 || Item.Base.MoneyFlag == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsExist(UInt32 TypeId)
        {
            IffMascotData Mascot = new IffMascotData();

            if (!LoadMascot(TypeId, ref Mascot))
            {
                return false;
            }
            return Convert.ToBoolean(Mascot.Base.Enabled);
        }


        public IffMascotData GetItem(UInt32 ID)
        {
            IffMascotData Mascot = new IffMascotData();

            if (!LoadMascot(ID, ref Mascot))
            {
                return Mascot;
            }
            return Mascot;
        }
        public bool LoadMascot(UInt32 ID, ref IffMascotData Mascot)
        {
            if (!TryGetValue(ID, out Mascot))
            {
                return false;
            }
            return true;
        }
    }
}
