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
            var msg = $"{Money} $ for the specific category is too high. Try again by choose another category of usage or you can choose this composition above with {price} $";
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
                case 1027: description = "Εφόσον σας ενδιαφέρει απλή χρήση Internet, ταινιών, εφαρμογών office, social media, διαδικτυακά μαθήματα, ή και κανένα browser game, το συγκεκριμένο PC build θα σας καλύψει στον μεγαλύτερο βαθμό."; break;
                case 1028: description = "Για απλή χρήση εφαρμογών γραφείου και πλοήγηση στο Internet, και εφόσον δεν μας ενδιαφέρουν ιδιαίτερα τα παιχνίδια, η αγορά υπολογιστή σε αυτή την κατηγορία μας επιτρέπει να έχουμε ένα εξαιρετικό σύστημα, που θα μας κρατήσει για χρόνια.στο μέλλον μπορούμε να αναβαθμίσουμε μέχρι και σε Intel i9 11ης γενιάς με την ίδια μητρική, χωρίς κανένα πρόβλημα."; break;
                case 1029: description = "Αυτό το σύστημα είναι προσανατολισμένο καθαρά προς το gaming και θα παίξει ελαφριά παιχνίδια τύπου LoL, Valorant, CS:GO, κλπ άνετα στα 1080p."; break;
                case 1031: description = "Αυτό το σύστημα είναι προσανατολισμένο καθαρά προς το gaming και θα παίξει οποιοδήποτε σύγχρονο παιχνίδι άνετα σε ανάλυση Full HD (1080p)."; break;
                case 1032: description = "Αυτό το σύστημα είναι προσανατολισμένο καθαρά προς το gaming και θα παίξει οποιοδήποτε σύγχρονο παιχνίδι άνετα σε ανάλυση Full HD, τις περισσότερες φορές σε υψηλά settings."; break;
                case 1033: description = "Αυτό το σύστημα είναι προσανατολισμένο καθαρά προς το gaming και θα παίξει οποιοδήποτε σύγχρονο παιχνίδι άνετα σε ανάλυση Full HD με τις μέγιστες ρυθμίσεις, και σε 1440p με low-medium settings."; break;
                case 1034: description = "Αυτός ο υπολογιστής είναι ένα καλό εισαγωγικό σύστημα για χρήση Photoshop, Premiere, προγραμμάτων μουσικής παραγωγής, και οποιαδήποτε άλλης εργασίας δεν απαιτεί ξεχωριστή κάρτα γραφικών.Η σύνθεση θα μπορούσε επίσης να χρησιμοποιηθεί ως υπολογιστής αποκλειστικά για χρήση γραφείου, με τις μέγιστες δυνατές επιδόσεις."; break;
                case 1035: description = "Αυτός ο υπολογιστής είναι ένα καλό μεσαίο σύστημα για απαιτητική χρήση Photoshop, Premiere, AutoCAD, προγραμμάτων μουσικής παραγωγής, και οποιαδήποτε άλλης εργασίας δεν απαιτεί ξεχωριστή κάρτα γραφικών.Η σύνθεση θα μπορούσε επίσης να χρησιμοποιηθεί ως υπολογιστής αποκλειστικά για χρήση γραφείου, με τις μέγιστες δυνατές επιδόσεις.";break;
                case 1036: description = "Αυτός ο υπολογιστής είναι ένα επαγγελματικό σύστημα για απαιτητική χρήση Photoshop, Premiere, AutoCAD, βαρύ video editing, προγραμμάτων μουσικής παραγωγής, και οποιαδήποτε άλλης εργασίας δεν απαιτεί ξεχωριστή κάρτα γραφικών.Η σύνθεση θα μπορούσε επίσης να χρησιμοποιηθεί ως υπολογιστής αποκλειστικά για χρήση γραφείου, με τις μέγιστες δυνατές επιδόσεις."; break;

            }

            return description;
        }



    }
}
