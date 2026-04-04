using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Hədəflər")]
    public Transform player;       // Oyunçunun mövqeyi
    public Transform escapePoint;  // Qaçıb gedəcəyi nöqtə

    [Header("Ayarlar")]
    public float speed = 5f;       // NPC-nin qaçış sürəti
    public float stopDistance = 2f;// Oyunçuya nə qədər yaxınlaşanda dayansın

    // Vəziyyətlər: 0 = Üstünə gəlir, 1 = Dialoqdadır, 2 = Qaçıb gedir
    private int currentState = 0; 

    void Update()
    {
        // 1-ci Mərhələ: Oyunçunun üstünə qaçmaq
        if (currentState == 0) 
        {
            // NPC-ni Oyunçuya doğru hərəkət etdirir
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            // Oyunçu ilə arasındakı məsafəni yoxlayır
            if (Vector3.Distance(transform.position, player.position) <= stopDistance)
            {
                currentState = 1; // Dialoq mərhələsinə keç
                Debug.Log("NPC gəldi çatdı! Dialoq başlayır...");
                
                // Test üçün: 4 saniyə sonra dialoq avtomatik bitsin deyə funksiya çağırırıq.
                // (Əsl oyunda bunu dialoq UI-dakı "Bağla" düyməsinə bağlayacağıq)
                Invoke("EndDialogue", 4f); 
            }
        }
        
        // 2-ci Mərhələ: Dialoq
        else if (currentState == 1) 
        {
            // NPC dayanır və gözləyir. Burada animasiya əlavə etmək olar.
        }
        
        // 3-cü Mərhələ: Qaçıb getmək
        else if (currentState == 2) 
        {
            // NPC-ni Qaçış nöqtəsinə doğru hərəkət etdirir
            transform.position = Vector3.MoveTowards(transform.position, escapePoint.position, speed * Time.deltaTime);
        }
    }

    // Dialoq bitəndə işə düşəcək funksiya
    public void EndDialogue()
    {
        Debug.Log("Dialoq bitdi! NPC uzaqlaşır...");
        currentState = 2; // Qaçma mərhələsinə keç
    }
}