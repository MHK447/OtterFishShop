using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

[RequireComponent(typeof(IAPListener))]
public class InAppPurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    public static InAppPurchaseManager Instance { get; private set; }

    // 상품 ID 정의
    public static class ProductIDs
    {
        // 소모품
        
        // 비소모품
        public const string REMOVE_ADS = "otterfishshop_noads_100";
    }

    // 상품 정보 매핑
    private Dictionary<string, ProductMetadata> productMetadata = new Dictionary<string, ProductMetadata>();
    
    // 인앱 결제 컨트롤러
    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    
    // 결제 결과 콜백
    private Action<Result, string> purchaseCallback;
    
    // 결제 결과 상태
    public enum Result
    {
        Success,
        Failure,
        Pending
    }
    
    // 구매 진행 중 상태
    private bool isPurchaseInProgress = false;
    
    // 초기화 여부
    public bool IsInitialized => storeController != null && extensionProvider != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePurchasing();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 인앱 결제 초기화
    public void InitializePurchasing()
    {
        if (IsInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // // 소모품 추가
        // builder.AddProduct(ProductIDs.GOLD_SMALL, ProductType.Consumable);
        // builder.AddProduct(ProductIDs.GOLD_MEDIUM, ProductType.Consumable);
        // builder.AddProduct(ProductIDs.GOLD_LARGE, ProductType.Consumable);
        
        // 비소모품 추가
        builder.AddProduct(ProductIDs.REMOVE_ADS, ProductType.NonConsumable);
        //builder.AddProduct(ProductIDs.VIP_PACKAGE, ProductType.NonConsumable);
        
        // 구독 상품 추가
        //builder.AddProduct(ProductIDs.VIP_SUBSCRIPTION, ProductType.Subscription);
        
        UnityPurchasing.Initialize(this, builder);
        Debug.Log("인앱 결제 초기화 시작...");
    }

    // 상품 구매 시도
    public void PurchaseProduct(string productId, Action<Result, string> callback = null)
    {
        if (!IsInitialized)
        {
            Debug.LogError("인앱 결제가 초기화되지 않았습니다.");
            callback?.Invoke(Result.Failure, "인앱 결제가 초기화되지 않았습니다.");
            return;
        }
        
        if (isPurchaseInProgress)
        {
            Debug.LogWarning("다른 구매가 진행 중입니다.");
            callback?.Invoke(Result.Failure, "다른 구매가 진행 중입니다.");
            return;
        }
        
        isPurchaseInProgress = true;
        purchaseCallback = callback;
        
        Product product = storeController.products.WithID(productId);
        
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"상품 구매 시도: {product.definition.id}, 가격: {product.metadata.localizedPriceString}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            isPurchaseInProgress = false;
            Debug.LogError($"상품을 구매할 수 없습니다: {productId}");
            purchaseCallback?.Invoke(Result.Failure, "상품을 찾을 수 없거나 구매할 수 없습니다.");
            purchaseCallback = null;
        }
    }

    // 구매 복원 (iOS용)
    public void RestorePurchases(Action<Result> callback = null)
    {
        if (!IsInitialized)
        {
            Debug.LogError("인앱 결제가 초기화되지 않았습니다.");
            callback?.Invoke(Result.Failure);
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || 
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("구매 복원 시도 (iOS)");
            var apple = extensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log($"구매 복원 결과: {result}");
                callback?.Invoke(result ? Result.Success : Result.Failure);
            });
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("구매 복원 (Android)은 자동으로 처리됩니다.");
            callback?.Invoke(Result.Success);
        }
        else
        {
            Debug.LogWarning("현재 플랫폼에서는 구매 복원이 지원되지 않습니다.");
            callback?.Invoke(Result.Failure);
        }
    }

    // 상품 가격 정보 조회
    public string GetLocalizedPrice(string productId)
    {
        if (!IsInitialized) return "n/a";
        
        Product product = storeController.products.WithID(productId);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
        return "n/a";
    }

    // 상품 정보 조회
    public ProductMetadata GetProductInfo(string productId)
    {
        if (productMetadata.ContainsKey(productId))
        {
            return productMetadata[productId];
        }
        return null;
    }

    // 상품이 이미 구매되었는지 확인 (비소모품)
    public bool IsProductPurchased(string productId)
    {
        if (!IsInitialized) return false;
        
        Product product = storeController.products.WithID(productId);
        return product != null && product.hasReceipt;
    }

    #region IStoreListener 인터페이스 구현
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        
        // 상품 메타데이터 캐싱
        foreach (var product in controller.products.all)
        {
            if (product.availableToPurchase)
            {
                productMetadata[product.definition.id] = product.metadata;
                Debug.Log($"상품 로드: {product.definition.id}, 가격: {product.metadata.localizedPriceString}");
            }
        }
        
        Debug.Log("인앱 결제 초기화 완료");
        
        // VIP 상태 업데이트
        UpdateVIPStatus();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"인앱 결제 초기화 실패: {error}, {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        bool validPurchase = true; // 영수증 검증 로직 추가 가능
        
        if (validPurchase)
        {
            // 구매 처리 성공
            string productId = args.purchasedProduct.definition.id;
            Debug.Log($"상품 구매 성공: {productId}");
            
            // 상품별 보상 처리
            GrantProductReward(productId);
            
            isPurchaseInProgress = false;
            purchaseCallback?.Invoke(Result.Success, productId);
            purchaseCallback = null;
            
            return PurchaseProcessingResult.Complete;
        }
        else
        {
            Debug.LogWarning("구매 검증 실패");
            isPurchaseInProgress = false;
            purchaseCallback?.Invoke(Result.Failure, "구매 검증 실패");
            purchaseCallback = null;
            
            return PurchaseProcessingResult.Pending;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"구매 실패: {product.definition.id}, 이유: {failureReason}");
        
        isPurchaseInProgress = false;
        purchaseCallback?.Invoke(Result.Failure, $"구매 실패: {failureReason}");
        purchaseCallback = null;
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"구매 실패: {product.definition.id}, 이유: {failureDescription.reason}, 메시지: {failureDescription.message}");
        
        isPurchaseInProgress = false;
        purchaseCallback?.Invoke(Result.Failure, $"구매 실패: {failureDescription.message}");
        purchaseCallback = null;
    }
    #endregion

    // 상품별 보상 처리
    private void GrantProductReward(string productId)
    {
        switch (productId)
        {
            // case ProductIDs.GOLD_SMALL:
            //     GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Cash, 1000);
            //     break;
                
            // case ProductIDs.GOLD_MEDIUM:
            //     GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Cash, 5000);
            //     break;
                
            // case ProductIDs.GOLD_LARGE:
            //     GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Cash, 10000);
            //     break;
                
            case ProductIDs.REMOVE_ADS:
                GameRoot.Instance.ShopSystem.IsVipProperty.Value = true;
                break;
        }
        
        GameRoot.Instance.UserData.Save();
    }
    
    // VIP 상태 업데이트 (앱 시작 시 비소모품 상태 체크)
    private void UpdateVIPStatus()
    {
        if (IsProductPurchased(ProductIDs.REMOVE_ADS))
        {
            GameRoot.Instance.ShopSystem.IsVipProperty.Value = true;
        }
    }
} 