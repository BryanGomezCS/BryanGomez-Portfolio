//
//  Extensions.swift
//  TriviaGameDemo
//
//  Created by Bryan Gomez on 10/16/24.
//

import Foundation
import SwiftUI

extension Text {
    func lilacTitle() -> some View {
        self.font(.title)
            .fontWeight(.heavy)
            .foregroundColor(Color("AccentColor"))
    }
}
