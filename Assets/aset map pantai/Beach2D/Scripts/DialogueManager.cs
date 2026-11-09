
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Dialog")]
    public GameObject dialoguePanel;
    public GameObject nextButtonObject;
    public TMP_Text dialogueText;
    public Button nextButton;

    [Header("Klik Dialog")]
    public AudioSource audioSource;
    public AudioClip suaraKlik;

    [Header("Maskot")]
    public Image mascotImage;
    public Sprite[] mascotSprites;
    public GameObject[] mascotObjects;

    [Header("Isi Percakapan")]
    [TextArea(2, 4)]
    public string[] dialogues;

    [Header("Karakter Gameplay dan quest")]
    public GameObject quest;

    private int currentIndex = 0;
    private bool dialogueFinished = false;

    void Start()
    {
        // Sembunyikan karakter gameplay selama tutorial
        if (quest != null)
            quest.SetActive(false);

        // Pastikan UI percakapan terlihat
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nextButtonObject != null)
            nextButtonObject.SetActive(true);

        // Tampilkan dialog pertama
        ShowDialogue();

        if (nextButton != null)
            nextButton.onClick.AddListener(NextDialogue);
    }

    public void NextDialogue()
    {
        if (dialogueFinished)
            return;

        // Jika masih ada dialog berikutnya
        if (currentIndex < dialogues.Length - 1)
        {
            currentIndex++;
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }

        if (audioSource != null && suaraKlik != null)
        {
            audioSource.PlayOneShot(suaraKlik);
        }
    }

    void ShowDialogue()
    {
        if (dialogueText != null &&
            currentIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentIndex];
        }

        if (mascotImage != null &&
            currentIndex < mascotSprites.Length)
        {
            mascotImage.sprite = mascotSprites[currentIndex];
            mascotImage.preserveAspect = true;
        }
    }

    void EndDialogue()
    {
        dialogueFinished = true;

        // Sembunyikan panel percakapan
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Sembunyikan tombol Next
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false);

        // Sembunyikan maskot tutorial
        if (mascotObjects != null)
        {
            foreach (GameObject mascot in mascotObjects)
            {
                if (mascot != null)
                    mascot.SetActive(false);
            }
        }

        // Munculkan player
        if (quest != null)
            quest.SetActive(true);

    }

    void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextDialogue);
    }
}
