using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    [SerializeField] private TMP_Text symbolText;
    [SerializeField] private Button cellButton;

    private GameManager gameManager;
    private int cellIndex;

    private void Awake()
    {
        EnsureComponents();
    }

    private void EnsureComponents()
    {
        if (cellButton == null) cellButton = GetComponent<Button>();
        if (symbolText == null) symbolText = GetComponentInChildren<TMP_Text>();
    }

    public void Setup(int index, GameManager manager)
    {
        cellIndex = index;
        gameManager = manager;

        EnsureComponents();

        if (cellButton != null)
        {
            cellButton.onClick.RemoveAllListeners();
            cellButton.onClick.AddListener(OnCellClicked);
        }
    }

    private void OnCellClicked()
    {
        if (gameManager != null)
        {
            gameManager.OnCellClicked(cellIndex);
        }
    }

    public void SetSymbol(string symbol)
    {
        EnsureComponents();
        if (symbolText != null)
        {
            symbolText.text = symbol;
        }
    }

    public void SetFaded(bool isFaded)
    {
        EnsureComponents();
        if (symbolText != null)
        {
            Color color = symbolText.color;
            color.a = isFaded ? 0.3f : 1f;
            symbolText.color = color;
        }
    }

    public void ResetCell()
    {
        EnsureComponents();
        if (symbolText != null)
        {
            symbolText.text = "";
            Color color = symbolText.color;
            color.a = 1f;
            symbolText.color = color;
        }
        if (cellButton != null)
        {
            cellButton.interactable = true;
        }
    }
}