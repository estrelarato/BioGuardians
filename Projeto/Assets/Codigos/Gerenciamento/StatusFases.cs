/*using UnityEngine;
using UnityEngine.UI;

public class StatusFases : MonoBehaviour
{
    [System.Serializable]
    public class VisualFase
    {
        public string chaveSave;       // Ex: "Cerebro"
        public Image imagemBotao;      // Componente Image do botão
        public Sprite spriteNormal;    // Imagem padrão
        public Sprite spriteConcluido; // Imagem (concluído)
    }

    public VisualFase[] fases;

    void OnEnable()
    {
        foreach (var fase in fases)
        {
            bool concluida = PlayerPrefs.GetInt(fase.chaveSave, 0) == 1;
            fase.imagemBotao.sprite = concluida ? fase.spriteConcluido : fase.spriteNormal;
        }
    }
}*/ 

using UnityEngine;
using UnityEngine.UI;

public class StatusFases : MonoBehaviour
{
    [System.Serializable]
    public class VisualFase
    {
        public string chaveSave;
        public Image imagemBotao;
        public Sprite spriteNormal;
        public Sprite spriteConcluido;
    }

    public VisualFase[] fases;

    void OnEnable()
    {
        AtualizarVisuais();
    }

    public void AtualizarVisuais()
    {
        foreach (var fase in fases)
        {
            bool concluida = PlayerPrefs.GetInt(fase.chaveSave, 0) == 1;
            fase.imagemBotao.sprite = concluida ? fase.spriteConcluido : fase.spriteNormal;
        }
    }

    // --- ATALHOS DE TESTE NO INSPECTOR ---
    [ContextMenu("TESTE: Concluir Todas as Fases")]
    public void TestarConcluirTodas()
    {
        foreach (var fase in fases)
        {
            PlayerPrefs.SetInt(fase.chaveSave, 1);
        }
        PlayerPrefs.Save();
        AtualizarVisuais();
    }

    [ContextMenu("TESTE: Resetar Todas as Fases")]
    public void TestarResetarTodas()
    {
        foreach (var fase in fases)
        {
            PlayerPrefs.SetInt(fase.chaveSave, 0);
        }
        PlayerPrefs.Save();
        AtualizarVisuais();
    }
}