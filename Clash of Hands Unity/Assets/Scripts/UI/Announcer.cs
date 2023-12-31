using System.Collections.Generic;
using ClashOfHands.Data;
using ClashOfHands.Systems;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class Announcer : MonoBehaviour
    {
        [SerializeField]
        private float _startDelay = 0.2f;

        [Header("Results")]
        [SerializeField]
        private TextMeshProUGUI _roundResultText;

        [SerializeField]
        private BounceScale _resultAnim;

        [SerializeField]
        private string _winText;

        [SerializeField]
        private string _loseText;

        [SerializeField]
        private string _tieText;

        [Header("Announcements")]
        [SerializeField]
        private TextMeshProUGUI _announcerText;

        [SerializeField]
        private BounceScale _announcerAnim;

        private AnnouncerData _announcerData;

        private TweenCallback _showAnnouncement;

        private TweenCallback _showResult;

        private TweenCallback _onCompletedCallback;

        private TweenCallback _hideTexts;

        private readonly List<string> _announcementStrings = new(16);
        private int _result;

        private int _announcementIterator;

        private Sequence _sequence;

        public void Initialize(AnnouncerData announcerData)
        {
            gameObject.SetActive(true);

            _announcerData = announcerData;
            _showAnnouncement = ShowAnnouncement;
            _showResult = ShowResult;
            _hideTexts = HideTexts;

            HideTexts();
        }

        public void BuildAnnouncements(CardData[] cards, int result, int playerIndex, out float duration)
        {
            duration = 0;
            _announcementIterator = 0;
            _result = result;
            _announcementStrings.Clear();

            var playerCard = cards[playerIndex];

            if (playerCard != null)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (i == playerIndex)
                        continue;

                    _announcementStrings.Add(_announcerData.GetAnnouncementFor(playerCard, cards[i]));
                }
            }
            else
            {
                _announcementStrings.Add(_announcerData.MissTurn);
            }

            _sequence = DOTween.Sequence();

            _sequence.AppendInterval(_startDelay);
            duration += _startDelay;

            for (int i = 0; i < _announcementStrings.Count; i++)
            {
                _sequence.AppendCallback(_showAnnouncement);
                _sequence.AppendInterval(_announcerAnim.Duration);
                duration += _announcerAnim.Duration;
            }

            _sequence.AppendCallback(_showResult);
            _sequence.AppendInterval(_resultAnim.Duration);
            duration += _resultAnim.Duration;

            _sequence.Pause();
        }

        public void PlayAnnouncement()
        {
            _sequence.Play();
        }

        private void ShowAnnouncement()
        {
            _roundResultText.gameObject.SetActive(false);

            _announcerText.SetText(_announcementStrings[_announcementIterator]);
            _announcerText.gameObject.SetActive(true);

            _announcerAnim.Bounce(null);
            _announcementIterator++;
        }

        private void ShowResult()
        {
            _announcerText.gameObject.SetActive(false);

            var result = _result switch
            {
                0 => _tieText,
                -1 => _loseText,
                _ => _winText
            };

            _roundResultText.SetText(result);
            _roundResultText.gameObject.SetActive(true);

            _resultAnim.Bounce(_hideTexts);

            PlayWinLossSFX(_result);
        }

        private void PlayWinLossSFX(int result)
        {
            switch (result)
            {
                case 1:
                    SoundManager.Instance.PlayWinSFX();
                    break;
                case -1:
                    SoundManager.Instance.PlayLoseSFX();
                    break;
            }
        }

        private void HideTexts()
        {
            _announcerText.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
        }
    }
}