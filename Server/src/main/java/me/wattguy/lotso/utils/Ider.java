package me.wattguy.lotso.utils;

import java.util.ArrayList;
import java.util.List;

public class Ider{

    private List<Integer> dump = new ArrayList<>();
    private Integer id = 0;

    public Integer next(){

        if (dump.size() > 0){

            int i = dump.get(0);
            dump.remove(0);
            return i;

        }

        id++;
        return id - 1;

    }

    public void add(Integer i){
        if (id < i) return;

        dump.add(i);
    }

}