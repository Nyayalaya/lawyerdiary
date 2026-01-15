/*!
 * DevLys/Kruti Dev Keyboard Layout
 * Direct character mapping for Hindi typing (like MS Word)
 */
(function (window) {
    'use strict';

    var DevLysKeyboard = {
        // Normal key mappings (without Shift)
        normalKeys: {
            // Numbers
            '1': '१', '2': '२', '3': '३', '4': '४', '5': '५',
            '6': '६', '7': '७', '8': '८', '9': '९', '0': '०',

            // Special on number row
            '-': '-', '=': 'ृ',

            // Top row (QWERTY)
            'q': 'ौ', 'w': 'ै', 'e': 'ा', 'r': 'ी', 't': 'ू',
            'y': 'ब', 'u': 'ह', 'i': 'ग', 'o': 'द', 'p': 'ज',
            '[': 'ड', ']': '़', '\\': 'ॉ',

            // Home row (ASDFGH)
            'a': 'ो', 's': 'े', 'd': '्', 'f': 'ि', 'g': 'ु',
            'h': 'प', 'j': 'र', 'k': 'क', 'l': 'त', ';': 'च',
            "'": 'ट',

            // Bottom row (ZXCVBN)
            'z': 'ं', 'x': 'म', 'c': 'न', 'v': 'व', 'b': 'ल',
            'n': 'स', 'm': ',',
            ',': ',', '.': '।', '/': 'य'
        },

        // Shift key mappings
        shiftKeys: {
            // Shift + Numbers
            '!': '!', '@': '@', '#': '्र', '$': 'र्', '%': 'ज्ञ',
            '^': 'त्र', '&': 'क्ष', '*': 'श्र', '(': '(', ')': ')',

            '_': 'ः', '+': 'ऋ',

            // Shift + Top row
            'Q': 'औ', 'W': 'ऐ', 'E': 'आ', 'R': 'ई', 'T': 'ऊ',
            'Y': 'भ', 'U': 'ङ', 'I': 'घ', 'O': 'ध', 'P': 'झ',
            '{': 'ढ', '}': 'ञ', '|': 'ऑ',

            // Shift + Home row
            'A': 'ओ', 'S': 'ए', 'D': 'अ', 'F': 'इ', 'G': 'उ',
            'H': 'फ', 'J': 'ऋ', 'K': 'ख', 'L': 'थ', ':': 'छ',
            '"': 'ठ',

            // Shift + Bottom row
            'Z': '', 'X': 'ण', 'C': '', 'V': '', 'B': '',
            'N': 'श', 'M': 'ष',
            '<': 'ऽ', '>': '।', '?': '?'
        },

        // Get Hindi character for given key
        getChar: function (key, isShift) {
            if (isShift) {
                return this.shiftKeys[key] || key;
            }
            return this.normalKeys[key] || key;
        },

        // Initialize keyboard for input element
        enable: function (inputElement) {
            var self = this;

            // Remove any existing listeners
            $(inputElement).off('keydown.devlys');

            // Add keydown event listener
            $(inputElement).on('keydown.devlys', function (e) {
                // Allow Ctrl, Alt, Meta key combinations
                if (e.ctrlKey || e.altKey || e.metaKey) {
                    return true;
                }

                // Allow navigation and special keys
                var allowedKeys = [
                    'Backspace', 'Delete', 'Tab', 'Enter', 'Escape',
                    'ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown',
                    'Home', 'End', 'PageUp', 'PageDown'
                ];

                if (allowedKeys.indexOf(e.key) !== -1) {
                    return true;
                }

                // Convert printable characters
                if (e.key.length === 1) {
                    e.preventDefault();

                    var hindiChar = self.getChar(e.key, e.shiftKey);

                    // Insert at cursor position
                    var input = this;
                    var start = input.selectionStart;
                    var end = input.selectionEnd;
                    var value = input.value;

                    input.value = value.substring(0, start) + hindiChar + value.substring(end);

                    // Set cursor position after inserted character
                    var newPos = start + hindiChar.length;
                    input.setSelectionRange(newPos, newPos);

                    // Trigger input event for any listeners
                    $(input).trigger('input');

                    return false;
                }
            });

            console.log('DevLys keyboard enabled for input');
        },

        // Disable keyboard for input element
        disable: function (inputElement) {
            $(inputElement).off('keydown.devlys');
            console.log('DevLys keyboard disabled for input');
        },

        // Enable for all elements matching selector
        enableAll: function (selector) {
            var self = this;
            $(selector).each(function () {
                self.enable(this);
            });
        }
    };

    // Expose to window
    window.DevLysKeyboard = DevLysKeyboard;

})(window);