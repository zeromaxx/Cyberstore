using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.Tools
{
    public static class Tools
    {
        private static readonly Entities db = new Entities();

        public static string MsgBuilder(int price, double money)
        {
            var msg = "No availabe composition with " + money + " $ but we built this one with " + price + " $";
            return msg;
        }
        public static string MsgOverRun(int price, double Money)
        {
            var msg = $"{Money} $ for the specific category is too high. Try again choosing another category, or you can choose this composition with {price} $";
            return msg;
        }

        public static void SalesAndStock(int? _id ,int quantity =1)
        {
            db.products.SingleOrDefault(p => p.id == _id).stock -= quantity;
            db.products.SingleOrDefault(p => p.id == _id).sales+=quantity;
            db.SaveChanges();
        }
        public static product GetNullProduct()
        {
            var nullProduct = db.products.SingleOrDefault(x => x.id == 64);
            motherboard NullProdMotherBoard = new motherboard()
            {
                socket = " ",
                chipset_vendor = " ",
                model = " ",
                pcie_slots = " ",
                ram_type = " ",
            };
            cpu NullCPU = new cpu()
            {
                socket = " ",
                cores = 0,
                model = " ",
                frequency = " ",
                chipset = " ",
                threads = 0
            };
            ram NullRam = new ram()
            {
                memory = " ",
                type = "",
                cash_latency = " ",
                frequency = " "

            };
            psu NullPSU = new psu()
            {
                connector = "",
                watt = 0
            };
            gpu NullGpu = new gpu()
            {
                vram = "",
                chipset = "",
                resolution = ""
            };
            hardDisc NullHardDisc = new hardDisc()
            {
                connector = "",
                capacity = "",
                frequency = "",
                type = ""
            };
            monitor NullMonitor = new monitor()
            {
                ScreenSize = 0,
                RefreshRate = "",
                Resolution = "",
                AspectRatio = ""

            };
            box NullBox = new box()
            {
                MotherboardSize = "",
                Color = ""
            };
            nullProduct.thisMotherboard = NullProdMotherBoard;
            nullProduct.thisCpu = NullCPU;
            nullProduct.thisGpu = NullGpu;
            nullProduct.thisHardDisc = NullHardDisc;
            nullProduct.thisRam = NullRam;
            nullProduct.thisPsu = NullPSU;
            nullProduct.thisMonitor = NullMonitor;
            nullProduct.thisBox = NullBox;
            return nullProduct;
        }

        public static string SuggestedDesktopDescription (int id)
        {
            string description = "";

            switch (id)
            {
                case 1027: description = "Ideal for simple internet usage, movies, social media."; break;
                case 1028: description = "This is no gaming PC; the Mini 5i is designed for straightforward productivity, with the processing power necessary for day-to-day tasks such as browsing the web and running Microsoft Office 365. Still, it can manage a bit of play after your work is done, like streaming some Netflix or playing low-intensity indie games."; break;
                case 1029: description = "If you want an all-in-one system that is ready to go straight out of the box, the 2021 iMac is a fantastic choice, arguably the very best option on the market right now."; break;
                case 1031: description = "It certainly won't be for everyone, but sometimes you just want the best of the best, the absolute cream of the crop. By our judgement, that's the MSI MEG Aegis Ti5: an almost ludicrously powerful gaming PC with a jaw-droppingly unique chassis."; break;
                case 1032: description = "It certainly won't be for everyone, but sometimes you just want the best of the best, the absolute cream of the crop. By our judgement, that's the MSI MEG Aegis Ti5: an almost ludicrously powerful gaming PC with a jaw-droppingly unique chassis."; break;
                case 1033: description = "With a broad range of configurations to suit every budget, the XPS desktop starts at $750 and goes all the way up to a $2,430 powerhouse equipped with the latest components that will be perfect for demanding workloads like video editing."; break;
                case 1034: description = "If there's one desktop computer we'd recommend for digital artists, it is without a doubt the Lenovo Yoga A940. Lenovo's answer to the iMac, this is a feature-rich all-in-one PC with a thoughtful design that is sure to appeal to creative souls."; break;
                case 1035: description = "The One i300 is likely to be a bit overkill for the average user, given its high-end internal components and steep entry price. Still, this is indisputably the most powerful compact form factor PC available right now."; break;
                case 1036: description = "A frankly inspired piece of computer hardware engineering , the Corsair One i300 is a marvelously compact workstation computer that leverages some smart internal design choices to offer top-tier performance"; break;

            }

            return description;
        }



    }
}
