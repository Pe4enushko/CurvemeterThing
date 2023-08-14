    /// <summary>
    /// Список возможных событий при обучении
    /// </summary>
    public enum CurvemeterEvents { 
        Взял,
        Провел_линию
    }
    /// <summary>
    /// Список возможных ошибок при сдаче теста
    /// </summary>
    public enum CurvemeterErrors { 
        Не_провел_линию,
        Провёл_дальше_чем_надо
    }