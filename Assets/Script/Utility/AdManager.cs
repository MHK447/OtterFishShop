using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

public class AdManager : MonoBehaviour
{
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    private string _adUnitId = "ca-app-pub-4348570103813665/2176085665";

    private bool IsInterAdLoaded = false;
    private bool IsRewardAdLoaded = false;
    private bool isInitialized = false;
    private bool isLoadingRewardedAd = false;
    private bool isLoadingInterstitialAd = false;

    private string InitadUnitId = "ca-app-pub-4348570103813665/4816058422"; // 테스트 전면 광고 단위 ID

    // 연속 로드 실패 시 재시도 간격을 점진적으로 늘리기 위한 변수
    private int rewardedAdRetryCount = 0;
    private int interstitialAdRetryCount = 0;
    private const int MAX_RETRY_COUNT = 5;
    private const float INITIAL_RETRY_DELAY = 1f;

    void Start()
    {
        // 초기화를 지연시켜 앱 시작 시 리소스 경합 방지
        GameRoot.Instance.WaitTimeAndCallback(2f, InitializeAds);
    }

    private void InitializeAds()
    {
        try
        {
            // 네트워크 상태 확인
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("네트워크 연결이 없습니다. 광고 초기화 지연됩니다.");
                // 5초 후 다시 시도
                GameRoot.Instance.WaitTimeAndCallback(5f, InitializeAds);
                return;
            }

            // 이미 초기화된 경우 중복 초기화 방지
            if (isInitialized) return;

            MobileAds.Initialize(initStatus => {
                isInitialized = true;
                Debug.Log("광고 SDK 초기화 완료");
                
                // 초기화 성공 후 광고 로드
                LoadRewardedAd();
                LoadInterstitialAd();
            });
        }
        catch (Exception e)
        {
            Debug.LogError("광고 초기화 중 오류: " + e.Message);
            // 오류 발생 시 10초 후 다시 시도
            GameRoot.Instance.WaitTimeAndCallback(10f, InitializeAds);
        }
    }

    public void LoadInterstitialAd()
    {
        if (isLoadingInterstitialAd) return; // 중복 로드 방지
        isLoadingInterstitialAd = true;
        
        try
        {
            // 이전 광고 정리
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            Debug.Log("전면 광고 로딩 시작");

            var adRequest = new AdRequest();
            
            // 테스트 장치 설정
            // AdRequest.Builder builder = new AdRequest.Builder();
            // builder.AddTestDevice("2077ef9a63d2b398840261c8221a0c9b");
            // adRequest = builder.Build();

            InterstitialAd.Load(InitadUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    isLoadingInterstitialAd = false;
                    
                    if (error != null || ad == null)
                    {
                        interstitialAdRetryCount++;
                        float retryDelay = INITIAL_RETRY_DELAY * Mathf.Pow(2, interstitialAdRetryCount);
                        retryDelay = Mathf.Min(retryDelay, 60f); // 최대 60초까지 지연
                        
                        Debug.LogError($"전면 광고 로드 실패 (시도 {interstitialAdRetryCount}/{MAX_RETRY_COUNT}): {error?.ToString()}");
                        
                        if (interstitialAdRetryCount < MAX_RETRY_COUNT)
                        {
                            Debug.Log($"{retryDelay}초 후 전면 광고 다시 로드 시도");
                            GameRoot.Instance.WaitTimeAndCallback(retryDelay, LoadInterstitialAd);
                        }
                        return;
                    }

                    interstitialAdRetryCount = 0; // 성공 시 재시도 카운트 초기화
                    _interstitialAd = ad;
                    IsInterAdLoaded = true;

                    Debug.Log("전면 광고 로드 성공");

                    RegisterInterstitialEventHandlers(ad);
                });
        }
        catch (Exception e)
        {
            isLoadingInterstitialAd = false;
            Debug.LogError("전면 광고 로드 중 예외 발생: " + e.Message);
            
            // 5초 후 다시 시도
            GameRoot.Instance.WaitTimeAndCallback(5f, LoadInterstitialAd);
        }
    }

    private void RegisterInterstitialEventHandlers(InterstitialAd ad)
    {
        // 광고가 닫혔을 때
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("전면 광고가 닫혔습니다.");
            IsInterAdLoaded = false;
            
            // 광고 닫힘 핸들러 호출 후 신규 광고 로드
            HandleInterstitialAdClosed();
        };
        
        // 광고 표시 실패 시
        ad.OnAdFullScreenContentFailed += (AdError error) => {
            Debug.LogError("전면 광고 표시 실패: " + error);
            IsInterAdLoaded = false;
            LoadInterstitialAd(); // 실패 시 즉시 새 광고 로드
        };
    }

    // 전면 광고 표시
    public void ShowInterstitialAd(System.Action onAdClosed = null)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("광고 SDK가 초기화되지 않았습니다. 광고 표시를 건너뜁니다.");
            onAdClosed?.Invoke();
            return;
        }
        
        if (IsInterAdLoaded && _interstitialAd != null && _interstitialAd.CanShowAd())
        {
            // 광고 닫힘 이벤트에 콜백 추가
            if (onAdClosed != null)
            {
                Action adClosedAction = null;
                adClosedAction = () => {
                    _interstitialAd.OnAdFullScreenContentClosed -= adClosedAction;
                    onAdClosed.Invoke();
                };
                
                _interstitialAd.OnAdFullScreenContentClosed += adClosedAction;
            }
            
            _interstitialAd.Show();
            Debug.Log("전면 광고 표시");
            IsInterAdLoaded = false;
        }
        else
        {
            Debug.Log("전면 광고가 준비되지 않았습니다.");
            onAdClosed?.Invoke(); // 광고가 없어도 콜백 호출
            
            // 광고가 로드되지 않은 상태라면 다시 로드 시도
            if (!isLoadingInterstitialAd)
            {
                LoadInterstitialAd();
            }
        }
    }

    public void HandleInterstitialAdClosed()
    {
        // 다음 광고를 위해 새 광고 로드
        GameRoot.Instance.WaitTimeAndCallback(0.5f, LoadInterstitialAd);
    }

    public void LoadRewardedAd()
    {
        if (isLoadingRewardedAd) return; // 중복 로드 방지
        isLoadingRewardedAd = true;
        
        try
        {
            // 이전 광고 정리
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            Debug.Log("리워드 광고 로딩 시작");

            var adRequest = new AdRequest();

            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    isLoadingRewardedAd = false;
                    
                    if (error != null || ad == null)
                    {
                        rewardedAdRetryCount++;
                        float retryDelay = INITIAL_RETRY_DELAY * Mathf.Pow(2, rewardedAdRetryCount);
                        retryDelay = Mathf.Min(retryDelay, 60f); // 최대 60초까지 지연
                        
                        Debug.LogError($"리워드 광고 로드 실패 (시도 {rewardedAdRetryCount}/{MAX_RETRY_COUNT}): {error?.ToString()}");
                        
                        if (rewardedAdRetryCount < MAX_RETRY_COUNT)
                        {
                            Debug.Log($"{retryDelay}초 후 리워드 광고 다시 로드 시도");
                            GameRoot.Instance.WaitTimeAndCallback(retryDelay, LoadRewardedAd);
                        }
                        return;
                    }

                    rewardedAdRetryCount = 0; // 성공 시 재시도 카운트 초기화
                    _rewardedAd = ad;
                    IsRewardAdLoaded = true;
                    
                    Debug.Log("리워드 광고 로드 성공");

                    RegisterRewardedEventHandlers(ad);
                });
        }
        catch (Exception e)
        {
            isLoadingRewardedAd = false;
            Debug.LogError("리워드 광고 로드 중 예외 발생: " + e.Message);
            
            // 5초 후 다시 시도
            GameRoot.Instance.WaitTimeAndCallback(5f, LoadRewardedAd);
        }
    }

    // 리워드 광고 표시
    public void ShowRewardedAd(System.Action rewardAction)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("광고 SDK가 초기화되지 않았습니다. 보상을 즉시 지급합니다.");
            rewardAction?.Invoke();
            return;
        }
        
        if (IsRewardAdLoaded && _rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("리워드 광고 표시");
            
            // 보상 지급 핸들러 설정
            _rewardedAd.Show((Reward reward) => {
                Debug.Log($"사용자에게 보상이 지급되었습니다: {reward.Type}, {reward.Amount}");
                rewardAction?.Invoke();
            });
            
            IsRewardAdLoaded = false;
        }
        else
        {
            Debug.LogWarning("리워드 광고가 준비되지 않았습니다. 보상을 즉시 지급합니다.");
            rewardAction?.Invoke();
            
            // 광고가 로드되지 않은 상태라면 다시 로드 시도
            if (!isLoadingRewardedAd)
            {
                LoadRewardedAd();
            }
        }
    }

    // 광고가 닫혔을 때
    public void HandleRewardedAdClosed()
    {
        Debug.Log("리워드 광고가 닫혔습니다.");
        
        // 다음 광고를 위해 새 광고 로드 (지연 로딩으로 변경)
        GameRoot.Instance.WaitTimeAndCallback(0.5f, LoadRewardedAd);
    }

    private void RegisterRewardedEventHandlers(RewardedAd ad)
    {
        // 광고가 닫혔을 때
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("리워드 광고가 닫혔습니다.");
            IsRewardAdLoaded = false;
            HandleRewardedAdClosed();
        };
        
        // 광고 표시 실패 시
        ad.OnAdFullScreenContentFailed += (AdError error) => {
            Debug.LogError("리워드 광고 표시 실패: " + error);
            IsRewardAdLoaded = false;
            LoadRewardedAd(); // 실패 시 새 광고 로드
        };
        
        // 광고 수익이 발생했을 때
        ad.OnAdPaid += (AdValue adValue) => {
            Debug.Log($"리워드 광고 수익 발생: {adValue.Value} {adValue.CurrencyCode}");
        };
        
        // 광고 노출이 기록되었을 때
        ad.OnAdImpressionRecorded += () => {
            Debug.Log("리워드 광고 노출이 기록되었습니다.");
        };
        
        // 광고 클릭이 기록되었을 때
        ad.OnAdClicked += () => {
            Debug.Log("리워드 광고가 클릭되었습니다.");
        };
    }
    
    // 앱이 일시 정지되거나 재개될 때 호출
    private void OnApplicationPause(bool pause)
    {
        if (!pause) // 앱이 포그라운드로 돌아올 때
        {
            // 광고가 로드되지 않은 상태라면 새로 로드
            if (!IsRewardAdLoaded && !isLoadingRewardedAd)
            {
                GameRoot.Instance.WaitTimeAndCallback(1f, LoadRewardedAd);
            }
            
            if (!IsInterAdLoaded && !isLoadingInterstitialAd)
            {
                GameRoot.Instance.WaitTimeAndCallback(1f, LoadInterstitialAd);
            }
        }
    }
    
    // 스테이지 전환 중 광고 작업 일시 중지/재개
    public void PauseAdOperations(bool pause)
    {
        if (pause)
        {
            Debug.Log("광고 관련 작업 일시 중지 (스테이지 전환 중)");
            
            // 진행 중인 광고 로드 작업 표시 초기화
            isLoadingRewardedAd = false;
            isLoadingInterstitialAd = false;
            
            // 기존 광고 리소스 정리
            if (_rewardedAd != null)
            {
                try
                {
                    _rewardedAd.Destroy();
                    _rewardedAd = null;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("리워드 광고 정리 중 오류: " + e.Message);
                }
            }
            
            if (_interstitialAd != null)
            {
                try
                {
                    _interstitialAd.Destroy();
                    _interstitialAd = null;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("전면 광고 정리 중 오류: " + e.Message);
                }
            }
            
            IsRewardAdLoaded = false;
            IsInterAdLoaded = false;
        }
        else
        {
            Debug.Log("광고 관련 작업 재개");
            
            // 재시도 카운트 초기화
            rewardedAdRetryCount = 0;
            interstitialAdRetryCount = 0;
            
            // 광고 다시 로드
            GameRoot.Instance.WaitTimeAndCallback(1f, () => {
                if (!isLoadingRewardedAd && !IsRewardAdLoaded)
                {
                    LoadRewardedAd();
                }
                
                GameRoot.Instance.WaitTimeAndCallback(1.5f, () => {
                    if (!isLoadingInterstitialAd && !IsInterAdLoaded)
                    {
                        LoadInterstitialAd();
                    }
                });
            });
        }
    }
}