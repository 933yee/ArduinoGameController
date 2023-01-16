using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject[] blocks;
    public GameObject[] hint_block_imgs;
    public GameObject[] hint_spawn_pos;
    private GameObject[] hints = new GameObject[5];
    // Start is called before the first frame update
    private int[] nextBlocks = new int[100];
    int pos = 0, cnt = 0, next = 0, hint_pos = 1;
    int hint_display_pos = 0;
    void Start()
    {
        shuffle();
        shuffle();
        shuffle();

        NewBlockHint();
        NewBlockHint();
        NewBlockHint();
        NewBlockHint();
    }

    public GameObject NewBlock()
    {
        // Debug.Log("NewBlock!");
        GameObject ret = Instantiate(blocks[nextBlocks[pos++]], transform.position, Quaternion.identity);
        if (pos >= nextBlocks.Length) pos = 0;
        cnt++;
        if (cnt == 6) shuffle();
        NewBlockHint();
        return ret;
    }
    private void hintMove()
    {
        for (int i = 0; i < 5; i++)
        {
            if (hints[i] != null)
            {
                hints[i].transform.position += Vector3.up * 3;
            }
        }
    }
    public void NewBlockHint()
    {
        hintMove();
        if (hints[hint_display_pos] != null) Destroy(hints[hint_display_pos]);
        hints[hint_display_pos++] = Instantiate(hint_block_imgs[nextBlocks[hint_pos]], hint_spawn_pos[nextBlocks[hint_pos]].transform.position, Quaternion.identity);
        if (hint_display_pos >= 5) hint_display_pos = 0;
        hint_pos++;
        if (hint_pos >= nextBlocks.Length) hint_pos = 0;
        // Debug.Log("hint spawn!");
    }
    public void shuffle()
    {
        int[] blocks_cnt = new int[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {

            int r = Random.Range(0, blocks.Length);
            while (blocks_cnt[r] == 1)
            {
                r = Random.Range(0, blocks.Length);
            }
            // Debug.Log(r);
            blocks_cnt[r] = 1;
            nextBlocks[next++] = r;
            if (next >= nextBlocks.Length) next = 0;

        }
        cnt = 0;
    }
}
