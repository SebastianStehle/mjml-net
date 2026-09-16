// Measures the render performance of the official MJML renderer.
//
// Usage: node bench.js <templatesDir> <warmup> <iterations> <outputFile>
const fs = require('fs');
const path = require('path');
const mjml2html = require('mjml');

const [templatesDir, warmupArg, iterationsArg, outputFile] = process.argv.slice(2);

const warmup = parseInt(warmupArg, 10);
const iterations = parseInt(iterationsArg, 10);

const options = {
    beautify: false,
    minify: false,
    validationLevel: 'skip',
};

async function render(template) {
    // mjml 4 is synchronous, mjml 5 returns a promise.
    return await Promise.resolve(mjml2html(template.source, { ...options, filePath: template.filePath }));
}

function elapsedMs(start) {
    return Number(process.hrtime.bigint() - start) / 1e6;
}

async function main() {
    const templates = fs.readdirSync(templatesDir)
        .filter(x => x.endsWith('.mjml'))
        .sort()
        .map(name => {
            const filePath = path.join(templatesDir, name);

            return { name, filePath, source: fs.readFileSync(filePath, 'utf8') };
        });

    // Cold render of every template, before anything is warmed up.
    const firstRenders = {};
    for (const template of templates) {
        const start = process.hrtime.bigint();
        await render(template);

        firstRenders[template.name] = { firstMs: elapsedMs(start) };
    }

    // Warm up across all templates, so that V8 has optimized all code paths before measuring.
    for (let i = 0; i < warmup; i++) {
        for (const template of templates) {
            await render(template);
        }
    }

    if (global.gc) {
        global.gc();
    }

    // Round-robin over the templates, so that effects over time (JIT, CPU frequency) are distributed evenly.
    const timings = templates.map(() => []);
    for (let i = 0; i < iterations; i++) {
        for (let t = 0; t < templates.length; t++) {
            const start = process.hrtime.bigint();
            await render(templates[t]);
            timings[t].push(elapsedMs(start));
        }
    }

    const results = templates.map((template, t) => ({ template: template.name, timings: timings[t], ...firstRenders[template.name] }));

    const version = require('mjml/package.json').version;

    fs.writeFileSync(outputFile, JSON.stringify({ version, node: process.version, results }));
}

main().catch(err => {
    console.error(err);
    process.exit(1);
});
