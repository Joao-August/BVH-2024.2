using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PerformanceData
{
    public List<TestResult> testResults = new List<TestResult>();
}

[System.Serializable]
public class TestResult
{
    public float tempoSemBVH;
    public float tempoComBVH;
    public float melhoriaPorcentagem;
    public string timestamp;
}

public class RaycastPerformanceTest : MonoBehaviour
{
    public Transform shootOrigin;  // Origem do tiro (ex: Câmera do jogador)
    public int numTests = 100;     // Número de testes para a média
    public LayerMask layerMask;    // Layer dos inimigos
    public float testInterval = 5f; // Tempo entre cada rodada de testes (segundos)

    private BVHGenerator bvhGenerator;
    private string filePath;
    private PerformanceData performanceData;

    void Start()
    {
        bvhGenerator = FindObjectOfType<BVHGenerator>(); // Encontra o BVH na cena
        filePath = Application.persistentDataPath + "/RaycastPerformance.json";

        // Carrega dados antigos se o arquivo já existir
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            performanceData = JsonUtility.FromJson<PerformanceData>(json);
        }
        else
        {
            performanceData = new PerformanceData();
        }

        InvokeRepeating(nameof(TestPerformance), 2f, testInterval); // Executa periodicamente
    }

    void TestPerformance()
    {
        float totalTimeWithoutBVH = 0;
        float totalTimeWithBVH = 0;

        for (int i = 0; i < numTests; i++)
        {
            Vector3 direction = Random.insideUnitSphere.normalized;  // Direção aleatória

            // Teste SEM BVH
            totalTimeWithoutBVH += MeasureWithoutBVH(shootOrigin.position, direction);

            // Teste COM BVH
            totalTimeWithBVH += MeasureWithBVH(shootOrigin.position, direction);
        }

        // Média dos tempos
        float avgWithoutBVH = totalTimeWithoutBVH / numTests;
        float avgWithBVH = totalTimeWithBVH / numTests;
        float improvement = ((avgWithoutBVH - avgWithBVH) / avgWithoutBVH) * 100;

        // Criar resultado do teste
        TestResult result = new TestResult
        {
            tempoSemBVH = avgWithoutBVH,
            tempoComBVH = avgWithBVH,
            melhoriaPorcentagem = improvement,
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // Adicionar ao histórico e salvar
        performanceData.testResults.Add(result);
        SaveToJSON();

        // Exibir resultados no Console
        UnityEngine.Debug.Log($"[JSON] Média SEM BVH: {avgWithoutBVH:F6}s | COM BVH: {avgWithBVH:F6}s | Melhorou {improvement:F2}%");
    }

    float MeasureWithoutBVH(Vector3 origin, Vector3 direction)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask);

        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalSeconds;
    }

    float MeasureWithBVH(Vector3 origin, Vector3 direction)
    {
        if (bvhGenerator == null) return 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        RaycastHit hit;
        bvhGenerator.RaycastBVH(origin, direction, out hit);

        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalSeconds;
    }

    void SaveToJSON()
    {
        string json = JsonUtility.ToJson(performanceData, true);
        File.WriteAllText(filePath, json);
    }
}
