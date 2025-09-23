//
//  Answer.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/16/24.
//

import Foundation

struct Answer: Identifiable {
    var id = UUID()
    var text: AttributedString
    var isCorrect: Bool
}
