// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WtnX2Oha2dLaWtnZ2GrHqGRvWrkYtP2Lr56OyciXjTwWclOT8f2CovOVDWcmS8+X6EaItoyGBJtvBAehQHpod6YycgRIKnJs/Dcw2/TjyPOtei7JGnnMvMQoSxqonnPg/Q8HK6b5Xb4vhnALysZVmWiZic39fHENrm0VRBCRyyxOxZ/LZ79vm9r3LXc4MEirT63XM128DCHDLRxHpX53jvkuJSJJktEdOCS9r2hzB8KcubW53YoL90DRezmG7gOeLxWnbusDEFgjAkwseILVeA9swAKP8TlGfaHtwdVstTRToPumlmQ33qr76zc/cs1zxnq8g+Juon4MH2aPMAKGLsH0MmXoWtn66NXe0fJekF4v1dnZ2d3Y2yIATRI/BHc6Z9rb2djZ");
        private static int[] order = new int[] { 1,13,3,9,6,8,12,9,10,10,10,12,12,13,14 };
        private static int key = 216;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
