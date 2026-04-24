namespace SmartHealthReport.Data;

public static class SampleData
{
    public static string GetJson()
    {
        return """
        {
          "Patient": { "Name": "Mr. Ram Mohan" },
          "ReportId": "250000514300377",
          "HealthScore": 88,
          "ReportUrl": "https://repots.armedu.in/0193258caod01dgwsd-214e-",
          "HealthSummaryParameters": [
            { "Name": "Electrolytes", "Icon": "\u26A1", "Previous": 100, "Current": 123, "Trend": "up" },
            { "Name": "Glucose", "Icon": "\uD83D\uDCA7", "Previous": 245, "Current": 385, "Trend": "up" },
            { "Name": "Kidney Function", "Icon": "\uD83D\uDCA7", "Previous": 12, "Current": 12, "Trend": "stable" },
            { "Name": "Lorem Ipsum", "Icon": "\uD83D\uDCA7", "Previous": 12, "Current": 12, "Trend": "stable" },
            { "Name": "Lorem Ipsum", "Icon": "\uD83D\uDCA7", "Previous": 34, "Current": 18, "Trend": "down" },
            { "Name": "Lorem Ipsum", "Icon": "\uD83D\uDCA7", "Previous": 34, "Current": 18, "Trend": "down" }
          ],
          "TestSummaryCards": [
            { "Name": "Glucose", "Status": "Normal", "Value": "132", "Unit": "mg/dL", "Range": "70.00-100.00", "Trend": "up" },
            { "Name": "Glucose (PP)", "Status": "Normal", "Value": "132", "Unit": "mg/dL", "Range": "70.00-100.00", "Trend": "up" },
            { "Name": "Liver Function", "Status": "Abnormal", "Value": "132", "Unit": "mg/dL", "Range": "70.00-100.00", "Trend": "down" },
            { "Name": "Iron", "Status": "Borderline", "Value": "132", "Unit": "mg/dL", "Range": "70.00-100.00", "Trend": "stable" },
            { "Name": "Pancreas", "Status": "Borderline", "Value": "132", "Unit": "mg/dL", "Range": "70.00-100.00", "Trend": "stable" }
          ],
          "ActionPlan": [
            {
              "ParameterName": "LDL",
              "Value": "145",
              "Description": "LDL cholesterol of 145 mg/dL is elevated and, in many people, associated with a higher long-term risk of heart disease.",
              "Suggestion": "Choose more idli, dosa, and uttapam made with whole grains or fermented batters (ragi, red rice, millet), plenty of sambar and rasam with lots of vegetables and lentils, snack on sprouts/sundal instead of fried items, use groundnut/gingelly oil instead of vanaspati, and keep ghee, fried snacks, bakery items, and red-meat dishes occasional while staying active and maintaining a healthy weight.",
              "Severity": "warning"
            },
            {
              "ParameterName": "Hs-CRP",
              
              "Description": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
              "Suggestion": "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
              "Severity": "critical"
            }
          ],
          "ClinicalData": [
            {
              "CategoryName": "Glucose",
              "OverallStatus": "Borderline",
              "Tests": [
                { "TestName": "Fasting Glucose Levels", "Result": "105", "Unit": "mg/dL", "Range": "<=10 : Normal | 100-125 : Pre Diabetes | >126 : Diabetes", "Level": "borderline" },
                { "TestName": "Post Prandial Glucose Levels", "Result": "130", "Unit": "mg/dL", "Range": "<=140 mg/dl : Normal | 100-125 : Pre Diabetes | >= 200 mg/dl : Diabetes", "Level": "normal" },
                { "TestName": "Glycosylated Hb", "Result": "6.20", "Unit": "%", "Range": "<= 5.6% : Normal | 5.7-6.4% : Prediabetes | >= 6.5% : Diabetes", "Level": "borderline" }
              ]
            },
            {
              "CategoryName": "Cholesterol",
              "OverallStatus": "Borderline",
              "Tests": [
                { "TestName": "Triglycerides (Fasting Sample)", "Result": "105", "Unit": "mg/dL", "Range": "<150 : Normal | 150-199 : Borderline | >=200 : High", "Level": "borderline" },
                { "TestName": "HDL Cholesterol", "Result": "130", "Unit": "mg/dL", "Range": ">=60 : Normal | 40-59 : Borderline | <40 : Low", "Level": "normal" },
                { "TestName": "LDL Cholesterol", "Result": "145", "Unit": "mg/dL", "Range": "<100 : Optimal | 100-129 : Near Optimal | 130-159 : Borderline", "Level": "borderline" }
              ]
            }
          ],
          "PersonalisedGuidance": [
            {
              "TestName": "Blood Sugar (Postprandial)",
              "Status": "Borderline",
              "Description": "It estimates the level of glucose in your blood after a meal. Ideally, a blood sample is taken 2 hours after you have orally ingested a set amount of sugar. Impaired glucose tolerance and prediabetes can be diagnosed using this test. This test is helpful in early diagnosis of type 2 diabetes.",
              "DietTips": ["Brown rice, ragi, whole wheat chapati, vegetable upma, vegetable pongal, rava dosa/idli", "Sambar/dal, curd/buttermilk/paneer", "Poriyal, Kootu, Salads, Cabbage, Okra, Beans, Leafy greens"],
              "LifestyleTips": ["Exercise regularly - helps boost your body's sensitivity to insulin", "Lose weight. Obesity increases your risk for diabetes.", "Avoid sweets and soft drinks. Don't add sugar in foods and drinks."]
            },
            {
              "TestName": "Calcium Total, Serum",
              "Status": "Normal",
              "Description": "Calcium is essential for bone health, muscle function, and nerve signaling. Your levels are within the normal range.",
              "DietTips": ["Milk, curd, paneer, cheese", "Ragi, sesame seeds, almonds", "Green leafy vegetables like amaranth, drumstick leaves"],
              "LifestyleTips": ["Get adequate sunlight for Vitamin D synthesis", "Weight-bearing exercises help maintain bone density", "Limit caffeine and carbonated drinks"]
            },
            {
              "TestName": "Creatinine, Serum",
              "Status": "Normal",
              "Description": "Creatinine is a waste product from normal muscle metabolism. Monitoring creatinine levels helps assess kidney function and detect potential issues early.",
              "DietTips": ["Dal, curd, paneer, eggs in moderation", "Whole grains, ragi, jowar", "Fresh fruits and vegetables"],
              "LifestyleTips": ["Stay well hydrated throughout the day", "Avoid excessive protein supplements", "Regular moderate exercise helps maintain kidney health"]
            },
            {
              "TestName": "Haemoglobin Hb",
              "Status": "Normal",
              "Description": "Haemoglobin carries oxygen from your lungs to the rest of your body. Healthy levels are essential for energy and overall vitality.",
              "DietTips": ["Spinach, beetroot, pomegranate, dates", "Jaggery, ragi, green leafy vegetables", "Iron-rich foods with vitamin C for better absorption"],
              "LifestyleTips": ["Include iron-rich foods in your daily diet", "Avoid tea or coffee immediately after meals", "Regular health check-ups to monitor levels"]
            }
          ],
          "DoctorQuestions": [
            "Is further imaging (like CT, MRI, ultrasound, or repeat X-ray) needed to clarify this result?",
            "Can I continue my usual work and routine, or do I need rest or any restrictions?",
            "My HbA1c went up slightly. Should we adjust my diet or consider medication?",
            "Do my liver enzymes indicate I should change any of my current supplements?"
          ]
        }
        """;
    }
}
