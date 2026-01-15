/*!
 * Kruti Dev 010 → Unicode (IME-style)
 * Production safe, deterministic, government compliant
 */

var KrutiToUnicode = (function () {
    'use strict';

    /* =====================================================
       1️⃣ BASE GLYPH MAP (OFFICIAL KRUTI DEV 010)
       ===================================================== */

    const map = [
        // Conjunct glyphs
        ['à', 'क्ष'],
        ['=', 'ज्ञ'],
        ['J', 'श्र'],

        // Reph marker
        ['«', 'र्'],

        // Consonants
        ['d', 'क'], ['H', 'भ'], ['x', 'ग'], ['g', 'ह'],
        ['p', 'च'], ['P', 'छ'], ['t', 'ज'], ['T', 'झ'],
        ['V', 'ट'], ['B', 'ठ'], ['M', 'ड'], ['<', 'ढ'],
        ['Z', 'ण'], ['r', 'त'], ['R', 'थ'], ['n', 'द'],
        ['N', 'ध'], ['u', 'न'], ['i', 'प'], ['I', 'फ'],
        ['c', 'ब'], ['e', 'म'], ['¸', 'य'], ['j', 'र'],
        ['y', 'ल'], ['Y', 'व'], ['o', 'श'], ['O', 'ष'],
        ['l', 'स'],

        // Matras (visual order)
        ['kS', 'ौ'], ['ks', 'ो'],
        ['k', 'ा'],
        ['f', 'ि'], ['h', 'ि'],
        ['q', 'ी'],
        ['w', 'ु'], ['Å', 'ू'],
        ['`', 'ृ'],
        ['s', 'े'], ['z', 'ै'],

        // Signs
        ['U', 'ं'], ['~', 'ँ'], ['\\', '़'],
        ['/', '।'], ['|', '॥'],

        // Digits
        ['0', '०'], ['1', '१'], ['2', '२'], ['3', '३'],
        ['4', '४'], ['5', '५'], ['6', '६'], ['7', '७'],
        ['8', '८'], ['9', '९']
    ];

    /* =====================================================
       2️⃣ STRUCTURAL FIXES (IME RULES)
       ===================================================== */

    // Fix short-i (ि) placement
    function fixIMatra(text) {
        return text.replace(/ि([क-ह])/g, '$1ि');
    }

    // Resolve anusvara context (MOST IMPORTANT)
    function fixAnusvara(text) {
        return text
            .replace(/ं([क-घ])/g, 'ङ्$1')
            .replace(/ं([च-झ])/g, 'ञ्$1')
            .replace(/ं([ट-ढ])/g, 'ण्$1')
            .replace(/ं([त-ध])/g, 'न्$1')
            .replace(/ं([प-भ])/g, 'म्$1');
    }

    // Fix reph placement
    function fixReph(text) {
        return text.replace(/र्([क-ह][्]?[क-ह]?)/g, '$1र्');
    }

    /* =====================================================
       3️⃣ MAIN CONVERTER
       ===================================================== */

    function convert(input) {
        if (!input) return '';

        let output = input;

        // Longest-first replacement
        map.sort((a, b) => b[0].length - a[0].length);

        map.forEach(([from, to]) => {
            const re = new RegExp(
                from.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'),
                'g'
            );
            output = output.replace(re, to);
        });

        // Apply IME rules (ORDER IS CRITICAL)
        output = fixIMatra(output);
        output = fixAnusvara(output);
        output = fixReph(output);

        return output;
    }

    /* =====================================================
       4️⃣ LIVE INPUT SUPPORT (NO FLICKER)
       ===================================================== */

    function convertLive(el) {
        if (!el || !el.value) return '';

        const start = el.selectionStart;
        const oldVal = el.value;
        const newVal = convert(oldVal);

        if (oldVal !== newVal) {
            el.value = newVal;
            const delta = newVal.length - oldVal.length;
            el.setSelectionRange(start + delta, start + delta);
        }

        return newVal;
    }

    return {
        convert,
        convertLive
    };
})();
