using System.Drawing;

namespace NetcoreSaas.Application.Helpers
{
    public static class TailwindColors
    {
        public static Color ErrorBackground;
        public static Color WarningBackground;
        public static Color SuccessBackground;
        public static Color InfoBackground;
        public static Color ErrorText;
        public static Color WarningText;
        public static Color SuccessText;
        public static Color InfoText;
        
        static TailwindColors()
        {
            InfoText = BlueGray600;
            SuccessText = Teal600;
            WarningText = Yellow600;
            ErrorText = Rose50;
            InfoBackground = BlueGray50;
            SuccessBackground = Teal50;
            WarningBackground = Yellow50;
            ErrorBackground = Rose50;
        }

        public static Color BlueGray50 = ColorTranslator.FromHtml("#F8FAFC");
        public static Color BlueGray100 = ColorTranslator.FromHtml("#F1F5F9");
        public static Color BlueGray200 = ColorTranslator.FromHtml("#E2E8F0");
        public static Color BlueGray300 = ColorTranslator.FromHtml("#CBD5E1");
        public static Color BlueGray400 = ColorTranslator.FromHtml("#94A3B8");
        public static Color BlueGray500 = ColorTranslator.FromHtml("#64748B");
        public static Color BlueGray600 = ColorTranslator.FromHtml("#475569");
        public static Color BlueGray700 = ColorTranslator.FromHtml("#334155");
        public static Color BlueGray800 = ColorTranslator.FromHtml("#1E293B");
        public static Color BlueGray900 = ColorTranslator.FromHtml("#0F172A");

        public static Color CoolGray50 = ColorTranslator.FromHtml("#F9FAFB");
        public static Color CoolGray100 = ColorTranslator.FromHtml("#F3F4F6");
        public static Color CoolGray200 = ColorTranslator.FromHtml("#E5E7EB");
        public static Color CoolGray300 = ColorTranslator.FromHtml("#D1D5DB");
        public static Color CoolGray400 = ColorTranslator.FromHtml("#9CA3AF");
        public static Color CoolGray500 = ColorTranslator.FromHtml("#6B7280");
        public static Color CoolGray600 = ColorTranslator.FromHtml("#4B5563");
        public static Color CoolGray700 = ColorTranslator.FromHtml("#374151");
        public static Color CoolGray800 = ColorTranslator.FromHtml("#1F2937");
        public static Color CoolGray900 = ColorTranslator.FromHtml("#111827");

        public static Color Gray50 = ColorTranslator.FromHtml("#FAFAFA");
        public static Color Gray100 = ColorTranslator.FromHtml("#F4F4F5");
        public static Color Gray200 = ColorTranslator.FromHtml("#E4E4E7");
        public static Color Gray300 = ColorTranslator.FromHtml("#D4D4D8");
        public static Color Gray400 = ColorTranslator.FromHtml("#A1A1AA");
        public static Color Gray500 = ColorTranslator.FromHtml("#71717A");
        public static Color Gray600 = ColorTranslator.FromHtml("#52525B");
        public static Color Gray700 = ColorTranslator.FromHtml("#3F3F46");
        public static Color Gray800 = ColorTranslator.FromHtml("#27272A");
        public static Color Gray900 = ColorTranslator.FromHtml("#18181B");

        public static Color TrueGray50 = ColorTranslator.FromHtml("#FAFAFA");
        public static Color TrueGray100 = ColorTranslator.FromHtml("#F5F5F5");
        public static Color TrueGray200 = ColorTranslator.FromHtml("#E5E5E5");
        public static Color TrueGray300 = ColorTranslator.FromHtml("#D4D4D4");
        public static Color TrueGray400 = ColorTranslator.FromHtml("#A3A3A3");
        public static Color TrueGray500 = ColorTranslator.FromHtml("#737373");
        public static Color TrueGray600 = ColorTranslator.FromHtml("#525252");
        public static Color TrueGray700 = ColorTranslator.FromHtml("#404040");
        public static Color TrueGray800 = ColorTranslator.FromHtml("#262626");
        public static Color TrueGray900 = ColorTranslator.FromHtml("#171717");

        public static Color WarmGray50 = ColorTranslator.FromHtml("#FAFAF9");
        public static Color WarmGray100 = ColorTranslator.FromHtml("#F5F5F4");
        public static Color WarmGray200 = ColorTranslator.FromHtml("#E7E5E4");
        public static Color WarmGray300 = ColorTranslator.FromHtml("#D6D3D1");
        public static Color WarmGray400 = ColorTranslator.FromHtml("#A8A29E");
        public static Color WarmGray500 = ColorTranslator.FromHtml("#78716C");
        public static Color WarmGray600 = ColorTranslator.FromHtml("#57534E");
        public static Color WarmGray700 = ColorTranslator.FromHtml("#44403C");
        public static Color WarmGray800 = ColorTranslator.FromHtml("#292524");
        public static Color WarmGray900 = ColorTranslator.FromHtml("#1C1917");

        public static Color Red50 = ColorTranslator.FromHtml("#FEF2F2");
        public static Color Red100 = ColorTranslator.FromHtml("#FEE2E2");
        public static Color Red200 = ColorTranslator.FromHtml("#FECACA");
        public static Color Red300 = ColorTranslator.FromHtml("#FCA5A5");
        public static Color Red400 = ColorTranslator.FromHtml("#F87171");
        public static Color Red500 = ColorTranslator.FromHtml("#EF4444");
        public static Color Red600 = ColorTranslator.FromHtml("#DC2626");
        public static Color Red700 = ColorTranslator.FromHtml("#B91C1C");
        public static Color Red800 = ColorTranslator.FromHtml("#991B1B");
        public static Color Red900 = ColorTranslator.FromHtml("#7F1D1D");

        public static Color Orange50 = ColorTranslator.FromHtml("#FFF7ED");
        public static Color Orange100 = ColorTranslator.FromHtml("#FFEDD5");
        public static Color Orange200 = ColorTranslator.FromHtml("#FED7AA");
        public static Color Orange300 = ColorTranslator.FromHtml("#FDBA74");
        public static Color Orange400 = ColorTranslator.FromHtml("#FB923C");
        public static Color Orange500 = ColorTranslator.FromHtml("#F97316");
        public static Color Orange600 = ColorTranslator.FromHtml("#EA580C");
        public static Color Orange700 = ColorTranslator.FromHtml("#C2410C");
        public static Color Orange800 = ColorTranslator.FromHtml("#9A3412");
        public static Color Orange900 = ColorTranslator.FromHtml("#7C2D12");

        public static Color Amber50 = ColorTranslator.FromHtml("#FFFBEB");
        public static Color Amber100 = ColorTranslator.FromHtml("#FEF3C7");
        public static Color Amber200 = ColorTranslator.FromHtml("#FDE68A");
        public static Color Amber300 = ColorTranslator.FromHtml("#FCD34D");
        public static Color Amber400 = ColorTranslator.FromHtml("#FBBF24");
        public static Color Amber500 = ColorTranslator.FromHtml("#F59E0B");
        public static Color Amber600 = ColorTranslator.FromHtml("#D97706");
        public static Color Amber700 = ColorTranslator.FromHtml("#B45309");
        public static Color Amber800 = ColorTranslator.FromHtml("#92400E");
        public static Color Amber900 = ColorTranslator.FromHtml("#78350F");

        public static Color Yellow50 = ColorTranslator.FromHtml("#FEFCE8");
        public static Color Yellow100 = ColorTranslator.FromHtml("#FEF9C3");
        public static Color Yellow200 = ColorTranslator.FromHtml("#FEF08A");
        public static Color Yellow300 = ColorTranslator.FromHtml("#FDE047");
        public static Color Yellow400 = ColorTranslator.FromHtml("#FACC15");
        public static Color Yellow500 = ColorTranslator.FromHtml("#EAB308");
        public static Color Yellow600 = ColorTranslator.FromHtml("#CA8A04");
        public static Color Yellow700 = ColorTranslator.FromHtml("#A16207");
        public static Color Yellow800 = ColorTranslator.FromHtml("#854D0E");
        public static Color Yellow900 = ColorTranslator.FromHtml("#713F12");

        public static Color Lime50 = ColorTranslator.FromHtml("#F7FEE7");
        public static Color Lime100 = ColorTranslator.FromHtml("#ECFCCB");
        public static Color Lime200 = ColorTranslator.FromHtml("#D9F99D");
        public static Color Lime300 = ColorTranslator.FromHtml("#BEF264");
        public static Color Lime400 = ColorTranslator.FromHtml("#A3E635");
        public static Color Lime500 = ColorTranslator.FromHtml("#84CC16");
        public static Color Lime600 = ColorTranslator.FromHtml("#65A30D");
        public static Color Lime700 = ColorTranslator.FromHtml("#4D7C0F");
        public static Color Lime800 = ColorTranslator.FromHtml("#3F6212");
        public static Color Lime900 = ColorTranslator.FromHtml("#365314");

        public static Color Green50 = ColorTranslator.FromHtml("#F0FDF4");
        public static Color Green100 = ColorTranslator.FromHtml("#DCFCE7");
        public static Color Green200 = ColorTranslator.FromHtml("#BBF7D0");
        public static Color Green300 = ColorTranslator.FromHtml("#86EFAC");
        public static Color Green400 = ColorTranslator.FromHtml("#4ADE80");
        public static Color Green500 = ColorTranslator.FromHtml("#22C55E");
        public static Color Green600 = ColorTranslator.FromHtml("#16A34A");
        public static Color Green700 = ColorTranslator.FromHtml("#15803D");
        public static Color Green800 = ColorTranslator.FromHtml("#166534");
        public static Color Green900 = ColorTranslator.FromHtml("#14532D");

        public static Color Emerald50 = ColorTranslator.FromHtml("#ECFDF5");
        public static Color Emerald100 = ColorTranslator.FromHtml("#D1FAE5");
        public static Color Emerald200 = ColorTranslator.FromHtml("#A7F3D0");
        public static Color Emerald300 = ColorTranslator.FromHtml("#6EE7B7");
        public static Color Emerald400 = ColorTranslator.FromHtml("#34D399");
        public static Color Emerald500 = ColorTranslator.FromHtml("#10B981");
        public static Color Emerald600 = ColorTranslator.FromHtml("#059669");
        public static Color Emerald700 = ColorTranslator.FromHtml("#047857");
        public static Color Emerald800 = ColorTranslator.FromHtml("#065F46");
        public static Color Emerald900 = ColorTranslator.FromHtml("#064E3B");

        public static Color Teal50 = ColorTranslator.FromHtml("#F0FDFA");
        public static Color Teal100 = ColorTranslator.FromHtml("#CCFBF1");
        public static Color Teal200 = ColorTranslator.FromHtml("#99F6E4");
        public static Color Teal300 = ColorTranslator.FromHtml("#5EEAD4");
        public static Color Teal400 = ColorTranslator.FromHtml("#2DD4BF");
        public static Color Teal500 = ColorTranslator.FromHtml("#14B8A6");
        public static Color Teal600 = ColorTranslator.FromHtml("#0D9488");
        public static Color Teal700 = ColorTranslator.FromHtml("#0F766E");
        public static Color Teal800 = ColorTranslator.FromHtml("#115E59");
        public static Color Teal900 = ColorTranslator.FromHtml("#134E4A");

        public static Color Cyan50 = ColorTranslator.FromHtml("#ECFEFF");
        public static Color Cyan100 = ColorTranslator.FromHtml("#CFFAFE");
        public static Color Cyan200 = ColorTranslator.FromHtml("#A5F3FC");
        public static Color Cyan300 = ColorTranslator.FromHtml("#67E8F9");
        public static Color Cyan400 = ColorTranslator.FromHtml("#22D3EE");
        public static Color Cyan500 = ColorTranslator.FromHtml("#06B6D4");
        public static Color Cyan600 = ColorTranslator.FromHtml("#0891B2");
        public static Color Cyan700 = ColorTranslator.FromHtml("#0E7490");
        public static Color Cyan800 = ColorTranslator.FromHtml("#155E75");
        public static Color Cyan900 = ColorTranslator.FromHtml("#164E63");

        public static Color LightBlue50 = ColorTranslator.FromHtml("#F0F9FF");
        public static Color LightBlue100 = ColorTranslator.FromHtml("#E0F2FE");
        public static Color LightBlue200 = ColorTranslator.FromHtml("#BAE6FD");
        public static Color LightBlue300 = ColorTranslator.FromHtml("#7DD3FC");
        public static Color LightBlue400 = ColorTranslator.FromHtml("#38BDF8");
        public static Color LightBlue500 = ColorTranslator.FromHtml("#0EA5E9");
        public static Color LightBlue600 = ColorTranslator.FromHtml("#0284C7");
        public static Color LightBlue700 = ColorTranslator.FromHtml("#0369A1");
        public static Color LightBlue800 = ColorTranslator.FromHtml("#075985");
        public static Color LightBlue900 = ColorTranslator.FromHtml("#0C4A6E");

        public static Color Blue50 = ColorTranslator.FromHtml("#EFF6FF");
        public static Color Blue100 = ColorTranslator.FromHtml("#DBEAFE");
        public static Color Blue200 = ColorTranslator.FromHtml("#BFDBFE");
        public static Color Blue300 = ColorTranslator.FromHtml("#93C5FD");
        public static Color Blue400 = ColorTranslator.FromHtml("#60A5FA");
        public static Color Blue500 = ColorTranslator.FromHtml("#3B82F6");
        public static Color Blue600 = ColorTranslator.FromHtml("#2563EB");
        public static Color Blue700 = ColorTranslator.FromHtml("#1D4ED8");
        public static Color Blue800 = ColorTranslator.FromHtml("#1E40AF");
        public static Color Blue900 = ColorTranslator.FromHtml("#1E3A8A");

        public static Color Indigo50 = ColorTranslator.FromHtml("#EEF2FF");
        public static Color Indigo100 = ColorTranslator.FromHtml("#E0E7FF");
        public static Color Indigo200 = ColorTranslator.FromHtml("#C7D2FE");
        public static Color Indigo300 = ColorTranslator.FromHtml("#A5B4FC");
        public static Color Indigo400 = ColorTranslator.FromHtml("#818CF8");
        public static Color Indigo500 = ColorTranslator.FromHtml("#6366F1");
        public static Color Indigo600 = ColorTranslator.FromHtml("#4F46E5");
        public static Color Indigo700 = ColorTranslator.FromHtml("#4338CA");
        public static Color Indigo800 = ColorTranslator.FromHtml("#3730A3");
        public static Color Indigo900 = ColorTranslator.FromHtml("#312E81");

        public static Color Violet50 = ColorTranslator.FromHtml("#F5F3FF");
        public static Color Violet100 = ColorTranslator.FromHtml("#EDE9FE");
        public static Color Violet200 = ColorTranslator.FromHtml("#DDD6FE");
        public static Color Violet300 = ColorTranslator.FromHtml("#C4B5FD");
        public static Color Violet400 = ColorTranslator.FromHtml("#A78BFA");
        public static Color Violet500 = ColorTranslator.FromHtml("#8B5CF6");
        public static Color Violet600 = ColorTranslator.FromHtml("#7C3AED");
        public static Color Violet700 = ColorTranslator.FromHtml("#6D28D9");
        public static Color Violet800 = ColorTranslator.FromHtml("#5B21B6");
        public static Color Violet900 = ColorTranslator.FromHtml("#4C1D95");

        public static Color Purple50 = ColorTranslator.FromHtml("#FAF5FF");
        public static Color Purple100 = ColorTranslator.FromHtml("#F3E8FF");
        public static Color Purple200 = ColorTranslator.FromHtml("#E9D5FF");
        public static Color Purple300 = ColorTranslator.FromHtml("#D8B4FE");
        public static Color Purple400 = ColorTranslator.FromHtml("#C084FC");
        public static Color Purple500 = ColorTranslator.FromHtml("#A855F7");
        public static Color Purple600 = ColorTranslator.FromHtml("#9333EA");
        public static Color Purple700 = ColorTranslator.FromHtml("#7E22CE");
        public static Color Purple800 = ColorTranslator.FromHtml("#6B21A8");
        public static Color Purple900 = ColorTranslator.FromHtml("#581C87");

        public static Color Fuchsia50 = ColorTranslator.FromHtml("#FDF4FF");
        public static Color Fuchsia100 = ColorTranslator.FromHtml("#FAE8FF");
        public static Color Fuchsia200 = ColorTranslator.FromHtml("#F5D0FE");
        public static Color Fuchsia300 = ColorTranslator.FromHtml("#F0ABFC");
        public static Color Fuchsia400 = ColorTranslator.FromHtml("#E879F9");
        public static Color Fuchsia500 = ColorTranslator.FromHtml("#D946EF");
        public static Color Fuchsia600 = ColorTranslator.FromHtml("#C026D3");
        public static Color Fuchsia700 = ColorTranslator.FromHtml("#A21CAF");
        public static Color Fuchsia800 = ColorTranslator.FromHtml("#86198F");
        public static Color Fuchsia900 = ColorTranslator.FromHtml("#701A75");

        public static Color Pink50 = ColorTranslator.FromHtml("#FDF2F8");
        public static Color Pink100 = ColorTranslator.FromHtml("#FCE7F3");
        public static Color Pink200 = ColorTranslator.FromHtml("#FBCFE8");
        public static Color Pink300 = ColorTranslator.FromHtml("#F9A8D4");
        public static Color Pink400 = ColorTranslator.FromHtml("#F472B6");
        public static Color Pink500 = ColorTranslator.FromHtml("#EC4899");
        public static Color Pink600 = ColorTranslator.FromHtml("#DB2777");
        public static Color Pink700 = ColorTranslator.FromHtml("#BE185D");
        public static Color Pink800 = ColorTranslator.FromHtml("#9D174D");
        public static Color Pink900 = ColorTranslator.FromHtml("#831843");

        public static Color Rose50 = ColorTranslator.FromHtml("#FFF1F2");
        public static Color Rose100 = ColorTranslator.FromHtml("#FFE4E6");
        public static Color Rose200 = ColorTranslator.FromHtml("#FECDD3");
        public static Color Rose300 = ColorTranslator.FromHtml("#FDA4AF");
        public static Color Rose400 = ColorTranslator.FromHtml("#FB7185");
        public static Color Rose500 = ColorTranslator.FromHtml("#F43F5E");
        public static Color Rose600 = ColorTranslator.FromHtml("#E11D48");
        public static Color Rose700 = ColorTranslator.FromHtml("#BE123C");
        public static Color Rose800 = ColorTranslator.FromHtml("#9F1239");
        public static Color Rose900 = ColorTranslator.FromHtml("#881337");
    }
}
