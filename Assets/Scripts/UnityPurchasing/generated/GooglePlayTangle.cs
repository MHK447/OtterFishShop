// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("sDM9MgKwMzgwsDMzMrGhea2n030lrf89Hec97RYDNlYFoUSu5kBkE4Wgrsmw+ADW1GgU90wqNSElXNmiUC+fvq27Ir4UWiak+EI3yRx8BIS7vLk6xZ+JrM25xqzQj5INqsEVyI9XTsVDrmTM+TuTNeA8pFSrj5bB7KCqDWa0lcnZORT3EKAVw8bB8ymL0vG8Vpd7A/n/ssiPYNaMt7F61RBLUCRZ0qDebPvG9Pziy7afNsypArAzEAI/NDsYtHq0xT8zMzM3MjFs8vTCITXtV4jXYgziC5waSHs6Xqadc8bE70kTK+Sm/g39V3WzkJMZJ0D4seBeB1HXjond7CpXavbGi+RQcnx6fioFT6Lnoa0+hLyCE2ybqZ6/+pqM0WNu4zAxMzIz");
        private static int[] order = new int[] { 1,6,13,12,13,7,13,8,11,13,10,11,12,13,14 };
        private static int key = 50;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
